using UnityEngine;

public class Utilities {
    public static Vector3 ClickPosition {
        get {

            if (Application.platform == RuntimePlatform.Android) {
                Touch touch = Input.GetTouch(0);
                return Camera.main.ScreenToWorldPoint(touch.position);
            } else {
                Camera camera = Camera.main;
                Vector3 mousePos = Input.mousePosition;
                Vector3 targetPosition = camera.ScreenToWorldPoint(mousePos);
                return targetPosition;
            }
        }
    }
}
