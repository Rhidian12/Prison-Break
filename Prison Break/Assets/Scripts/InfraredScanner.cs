using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class InfraredScanner : MonoBehaviour
{
    [SerializeField] private Material m_MaterialToApply;
    [SerializeField] private float m_ScanRange = 0f;
    [SerializeField] private float m_ScanAngle = 0f;
    [SerializeField] private float m_TimeToRevealObject = 0f;
    [SerializeField] private float m_Cooldown = 0f;

    private GameObject m_Player;
    private Transform m_InfraredSocket;
    private bool m_IsPickedUp = false;
    private float m_CooldownTimer = 0f;

    private struct RevealedObjectInformation
    {
        public RevealedObjectInformation(GameObject gameObject)
        {
            currentTimeRevealed = 0f;
            originalMaterial = null;
            meshRenderer = null;

            // Is the object the one with the visuals?
            if (gameObject.CompareTag("Visuals"))
            {
                meshRenderer = gameObject.GetComponent<MeshRenderer>();
                originalMaterial = meshRenderer.material;
            }
            // Search for the Visuals
            else
            {
                foreach (Transform child in gameObject.GetComponentsInChildren<Transform>())
                {
                    if (child.gameObject.CompareTag("Visuals"))
                    {
                        meshRenderer = child.GetComponent<MeshRenderer>();
                        originalMaterial = meshRenderer.material;
                        break;
                    }
                }
            }
        }

        public RevealedObjectInformation(GameObject gameObject, Material _originalMaterial, float time)
        {
            currentTimeRevealed = time;
            originalMaterial = _originalMaterial;
            meshRenderer = null;

            // Is the object the one with the visuals?
            if (gameObject.CompareTag("Visuals"))
                meshRenderer = gameObject.GetComponent<MeshRenderer>();
            // Search for the Visuals
            else
            {
                foreach (Transform child in gameObject.GetComponentsInChildren<Transform>())
                {
                    if (child.gameObject.CompareTag("Visuals"))
                    {
                        meshRenderer = child.GetComponent<MeshRenderer>();
                        break;
                    }
                }
            }
        }

        public RevealedObjectInformation(GameObject gameObject, float time)
        {
            currentTimeRevealed = time;
            originalMaterial = null;
            meshRenderer = null;

            // Is the object the one with the visuals?
            if (gameObject.CompareTag("Visuals"))
            {
                meshRenderer = gameObject.GetComponent<MeshRenderer>();
                originalMaterial = meshRenderer.material;
            }
            // Search for the Visuals
            else
            {
                foreach (Transform child in gameObject.GetComponentsInChildren<Transform>())
                {
                    if (child.gameObject.CompareTag("Visuals"))
                    {
                        meshRenderer = child.GetComponent<MeshRenderer>();
                        originalMaterial = meshRenderer.material;
                    }
                }
            }
        }


        public float currentTimeRevealed { get; set; }
        public Material originalMaterial { get; set; }
        public MeshRenderer meshRenderer { get; set; }
    }

    private struct GameObjectLayerInformation
    {
        public GameObjectLayerInformation(GameObject _gameObject, int _layer)
        {
            gameObject = _gameObject;
            originalLayer = _layer;
        }

        public GameObject gameObject { get; set; }
        public int originalLayer { get; set; }
    }

    private Dictionary<GameObject, RevealedObjectInformation> m_RevealedObjects = new Dictionary<GameObject, RevealedObjectInformation>();

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(m_MaterialToApply);

        foreach (GameObject gameObject in FindObjectsOfType<GameObject>())
        {
            // flag each gameObject that isn't the player to ignore the collision
            if (!gameObject.CompareTag("Player"))
            {
                Physics.IgnoreCollision(GetComponent<BoxCollider>(), GetComponent<Collider>());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_CooldownTimer >= 0f)
        {
            m_CooldownTimer -= Time.deltaTime;
            if (m_CooldownTimer <= 0f)
                Debug.Log("SCAN READY");
        }

        Dictionary<GameObject, RevealedObjectInformation> valuesToAdjust = new Dictionary<GameObject, RevealedObjectInformation>();
        List<GameObject> objectsToBeUnrevealed = new List<GameObject>();

        foreach (KeyValuePair<GameObject, RevealedObjectInformation> element in m_RevealedObjects)
        {
            valuesToAdjust[element.Key] = new RevealedObjectInformation(element.Value.meshRenderer.gameObject, element.Value.originalMaterial, element.Value.currentTimeRevealed + Time.deltaTime);

            if (valuesToAdjust[element.Key].currentTimeRevealed >= m_TimeToRevealObject)
            {
                element.Value.meshRenderer.material = element.Value.originalMaterial;
                objectsToBeUnrevealed.Add(element.Key);
            }
        }

        foreach (GameObject key in objectsToBeUnrevealed)
        {
            m_RevealedObjects[key].meshRenderer.material = m_RevealedObjects[key].originalMaterial;
            m_RevealedObjects.Remove(key);
            valuesToAdjust.Remove(key);
        }

        foreach(KeyValuePair<GameObject, RevealedObjectInformation> element in valuesToAdjust)
            m_RevealedObjects[element.Key] = element.Value;
    }

    private void OnTriggerEnter(Collider other)
    {
        // if we have picked the scanner up, ignore this function
        if (m_IsPickedUp)
            return;

        // is the player trying to pick the scanner up?
        if (other.CompareTag("Player")) // Player Tag should be the root
        {
            m_IsPickedUp = true;

            bool isRecursionDone = false;
            RecursivelySearchGameObject(other.gameObject, "InfraredSocket", ref isRecursionDone);

            // Make sure we have the player
            m_Player = other.gameObject;

            // parent the scanner to the infrared socket
            transform.parent = m_InfraredSocket;

            // Set this script in the PlayerController as the infrared scanner script
            m_Player.GetComponent<PlayerController>().SetInfraredScannerScript = this;

            transform.position = m_InfraredSocket.position;
        }
    }

    public void ScanAhead()
    {
        // Has the InfraredScanner been picked up?
        if (!m_IsPickedUp)
            return;

        // Is the InfraredScanner on cooldown?
        if (m_CooldownTimer >= 0f)
            return;

        // print a debug string
        Debug.Log("SCANNING");

        // Set the m_CooldownTimer
        m_CooldownTimer = m_Cooldown;

        // Get the direction to scan
        Vector3 direction = m_Player.transform.forward;

        // We want to ignore layer 2, which is the RaycastIgnore layer
        const int layerMask = ~(1 << 2);

        // Now that we have our scan field, we need to "reveal" everything there by giving it a red hue
        for (float i = -m_ScanAngle; i <= m_ScanAngle; i += 1f) // increase the angle by 5%
        {
            // Keep a list of layers we found
            List<GameObjectLayerInformation> foundLayers = new List<GameObjectLayerInformation>();

            RaycastHit raycastHit;
            Vector3 raycastDirection = Quaternion.AngleAxis(-i, Vector3.up) * direction;
            // Send a ray at an angle of i ([-m_ScanAngle, m_ScanAngle]), if we hit something we need to apply our red material to it for m_TimeRevealed seconds
            while (Physics.Raycast(m_Player.transform.position, raycastDirection, out raycastHit, m_ScanRange, layerMask))
            {
                GameObject gameObject = raycastHit.transform.gameObject;

                // Save the original gameObject Layer, and set the hit gameObject layer to the layerMask. Thus it will get ignored on the next pass
                foundLayers.Add(new GameObjectLayerInformation(gameObject, gameObject.layer));
                gameObject.layer = 2;

                // now redo the raycast to see if there's something behind the object we just hit
            }

            // Did we hit something?
            if (foundLayers.Count > 0)
            {
                foreach (GameObjectLayerInformation element in foundLayers)
                {
                    element.gameObject.layer = element.originalLayer; // reset the gameobjects their original layer
                    if (!m_RevealedObjects.ContainsKey(element.gameObject))
                        m_RevealedObjects.Add(element.gameObject, new RevealedObjectInformation(element.gameObject)); // add the gameObject to the hit gameObjects
                }
            }
        }

        List<GameObject> gameObjectsToRemove = new List<GameObject>();
        // For every game object hit, set their material to the Infrared_RevealedMaterial
        // But only if their material ISN'T the Infrared_RevealedMaterial already
        foreach (KeyValuePair<GameObject, RevealedObjectInformation> element in m_RevealedObjects)
        {
            // Is the object the one with the visuals?
            if (element.Key.CompareTag("Visuals"))
            {
                MeshRenderer meshRenderer = element.Key.GetComponent<MeshRenderer>();
                if (!meshRenderer.material.Equals(m_MaterialToApply))
                    meshRenderer.material = m_MaterialToApply;
                else
                    gameObjectsToRemove.Add(element.Key);
            }
            // Search for the Visuals
            else
            {
                foreach (Transform child in element.Key.GetComponentsInChildren<Transform>())
                {
                    if (child.gameObject.CompareTag("Visuals"))
                    {
                        MeshRenderer meshRenderer = child.gameObject.GetComponent<MeshRenderer>();
                        if (!meshRenderer.material.Equals(m_MaterialToApply))
                            meshRenderer.material = m_MaterialToApply;
                        else
                            gameObjectsToRemove.Add(element.Key);
                    }
                }
            }
        }

        foreach(GameObject gameObject in gameObjectsToRemove)
            m_RevealedObjects.Remove(gameObject);
    }

    private void RecursivelySearchGameObject(GameObject gameObject, string objectName, ref bool isRecursionDone)
    {
        if (isRecursionDone)
            return;

        if (gameObject.name.Equals(objectName))
        {
            m_InfraredSocket = gameObject.transform;
            isRecursionDone = true;
            return;
        }

        foreach (Transform transform in gameObject.transform.GetComponentInChildren<Transform>())
        {
            if (isRecursionDone)
                return;

            // Is this the infrared socket?
            if (transform.name.Equals(objectName))
            {
                m_InfraredSocket = transform;
                return;
            }

            foreach (Transform child in transform.GetComponentInChildren<Transform>())
            {
                if (isRecursionDone)
                    return;

                RecursivelySearchGameObject(child.gameObject, objectName, ref isRecursionDone);
            }
        }
    }
}
