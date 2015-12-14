using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapView : MonoBehaviour {

    public GameObject markerTemplate;
    public Transform centerObject;
    public RectTransform centerMarker;
    public Text countText;

    private Dictionary<GameObject, GameObject> pickupToMarker;
    private RectTransform contentTransform;

    void Start() {
        markerTemplate.SetActive(false);
        contentTransform = transform.GetChild(0).GetComponent<RectTransform>();
    }

    void Update() {
        Vector3 objPos = centerObject.position;
        contentTransform.anchoredPosition = new Vector2(-objPos.x * contentTransform.localScale.x, -objPos.z * contentTransform.localScale.y);
        centerMarker.anchoredPosition = new Vector2(objPos.x, objPos.z);
        centerMarker.localRotation = Quaternion.Euler(0, 0, -centerObject.rotation.eulerAngles.y);
    }

    public void SetPickups(HashSet<GameObject> objects) {

        pickupToMarker = new Dictionary<GameObject, GameObject>();

        foreach(GameObject obj in objects) {
            GameObject marker = (GameObject)Object.Instantiate(markerTemplate);
            marker.SetActive(true);
            marker.transform.SetParent(markerTemplate.transform.parent);
            RectTransform rect = marker.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(obj.transform.position.x, obj.transform.position.z);
            pickupToMarker[obj] = marker;
        }
        UpdateCount();
    }

    public void PickUp(GameObject pickup) {
        GameObject marker = pickupToMarker[pickup];
        Destroy(marker);
        pickupToMarker.Remove(pickup);
        UpdateCount();
    }

    private void UpdateCount() {
        int count = pickupToMarker.Count;
        if(count == 1) {
            countText.text = "1 cone";
        }
        else {
            countText.text = "" + pickupToMarker.Count + " cones";
        }
    }
}
