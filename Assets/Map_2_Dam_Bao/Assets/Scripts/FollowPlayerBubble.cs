using UnityEngine;

public class FollowPlayerBubble : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2.2f, 0f);

    void LateUpdate()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
            else
                return;
        }

        transform.position = target.position + offset;
        transform.localScale = Vector3.one;
    }
}