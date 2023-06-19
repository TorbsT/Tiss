using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Systems
{
    using ScriptableObjects;
    using Components;
    using UnityEngine;

    internal class LocomotionSystem : QuickSystem<Locomotion>
    {
        public void Move(GameObject go, Vector2 direction)
        {
            Locomotion loco = ComponentDict[go];
            loco.IntendedDirection = direction;
        }
        protected override void Tick(ICollection<Locomotion> components)
        {
            
        }
        private void FixedUpdate()
        {
            ICollection<Locomotion> components = CopyComponents();
            foreach (Locomotion loco in components)
            {
                LocomotionObject obj = loco.Object;
                Rigidbody2D rb = loco.Rb;

                Vector2 oldVelocity = rb.velocity;
                float oldMagnitude = oldVelocity.magnitude;

                float standstillMagnitude =
                    oldMagnitude - obj.Deceleration * Time.fixedDeltaTime;
                standstillMagnitude = Mathf.Max(0f, standstillMagnitude);

                if (loco.IntendedDirection == Vector2.zero)
                {
                    // Decelerate
                    if (oldMagnitude == 0f) return;
                    rb.velocity *= standstillMagnitude / oldMagnitude;
                }
                else
                {
                    // Accelerate
                    if (obj.Acceleration == 0f) return;

                    Vector2 walkVelocity =
                        oldVelocity + obj.Acceleration * Time.fixedDeltaTime *
                        loco.IntendedDirection.normalized;
                    float walkMagnitude = walkVelocity.magnitude;

                    if (walkMagnitude < standstillMagnitude)
                    {
                        // Walking direction slows down more than standing still
                        walkVelocity *= walkMagnitude / walkVelocity.magnitude;
                        rb.velocity = walkVelocity;
                    }
                    else
                    {
                        if (walkMagnitude > obj.BaseSpeed)
                        {
                            Vector2 newVelocity =
                                walkVelocity.normalized *
                                (oldMagnitude - obj.Deceleration);
                            if (newVelocity.magnitude < obj.BaseSpeed)
                                newVelocity = newVelocity.normalized * obj.BaseSpeed;
                            rb.velocity = newVelocity;
                        }
                        else
                        {
                            walkVelocity *= walkMagnitude / walkVelocity.magnitude;
                            rb.velocity = walkVelocity;
                        }
                    }

                    walkMagnitude = Mathf.Min(obj.BaseSpeed, walkMagnitude);
                    walkVelocity *= walkMagnitude / walkVelocity.magnitude;

                    rb.velocity = walkVelocity;
                }
            }
        }
    }
}
