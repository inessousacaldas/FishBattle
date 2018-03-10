using UnityEngine;

namespace Game.Preview
{
    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed = 5f;

        private SpriteAnimation sa;

        void Start()
        {
            sa = GetComponent<SpriteAnimation>();
        }

        void Update()
        {
            bool isMove = true;
            if (Input.GetKey(KeyCode.W))
            {
                sa.Move(0, moveSpeed);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                sa.Move(180, moveSpeed);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                sa.Move(-90, moveSpeed);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                sa.Move(90, moveSpeed);
            }
            else
            {
                isMove = false;
            }

            if (isMove)
            {
                sa.Action("run");
            }
            else
            {
                sa.Action("idle");
            }
        }
    }
}