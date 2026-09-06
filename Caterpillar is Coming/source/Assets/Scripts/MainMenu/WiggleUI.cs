using UnityEngine;

// 让 UI 元素轻轻上下浮动并摇摆，用于主界面装饰
public class WiggleUI : MonoBehaviour
{
    [Header("上下浮动")]
    public float bobAmplitude = 10f;
    public float bobSpeed = 2f;

    [Header("左右摇摆")]
    public float wiggleAngle = 5f;
    public float wiggleSpeed = 3f;

    private Vector3 startPos;
    private float offset;

    private void Start()
    {
        startPos = transform.localPosition;
        offset = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        float t = Time.unscaledTime + offset;
        transform.localPosition = startPos + new Vector3(0f, Mathf.Sin(t * bobSpeed) * bobAmplitude, 0f);
        transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(t * wiggleSpeed) * wiggleAngle);
    }
}
