using System;
using UnityEngine;


namespace XDaddy.Character
{
    [RequireComponent(typeof(Rigidbody2D))]
    //[RequireComponent(typeof(Collider2D))]
    public class CharacterController2D : MonoBehaviour
    {
        // Parameters        
        private Vector2 previousPosition;
        private Vector2 currentPosition;
        private Vector2 nextMovement;

        private Vector2 velocity = Vector2.zero;
        public Vector2 Velocity { get { return velocity; } set { velocity = value; } }

        private new Rigidbody2D rigidbody2D;
        public Rigidbody2D Rigidbody2D { get { return rigidbody2D; } }

        // Unity function
        void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();

            currentPosition = rigidbody2D.position;
            previousPosition = rigidbody2D.position;
        }
        void FixedUpdate()
        {
            previousPosition = rigidbody2D.position;
            currentPosition = previousPosition + nextMovement;
            Velocity = (currentPosition - previousPosition) / Time.deltaTime;

            rigidbody2D.MovePosition(currentPosition);
            nextMovement = Vector2.zero;
        }


        // Public function

        /// <summary>
        /// This moves a rigidbody and so should only be called from FixedUpdate or other Physics messages.
        /// </summary>
        /// <param name="movement">The amount moved in global coordinates relative to the rigidbody2D's position.</param>
        public void Move(Vector2 movement)
        {
            nextMovement += movement;
        }

        /// <summary>
        /// This moves the character without any implied velocity.
        /// </summary>
        /// <param name="position">The new position of the character in global space.</param>
        public void Teleport(Vector2 position)
        {
            Vector2 delta = position - currentPosition;
            previousPosition += delta;
            currentPosition = position;
            rigidbody2D.MovePosition(position);
        }


    }


}