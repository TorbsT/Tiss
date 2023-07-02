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

                float handicap = 1f - Mathf.Clamp(loco.Handicap, 0f, 1f);
                float allMultiplier = loco.AllMultiplier*handicap;
                float acceleration = obj.Acceleration*loco.AllMultiplier;
                float deceleration = obj.Deceleration*loco.AllMultiplier;
                float speed = obj.BaseSpeed*loco.AllMultiplier;

                Vector2 oldVelocity = rb.velocity;
                float oldMagnitude = oldVelocity.magnitude;

                loco.Handicap -= Time.fixedDeltaTime;

                float standstillMagnitude =
                    oldMagnitude - deceleration * Time.fixedDeltaTime;
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
                    if (acceleration == 0f) return;

                    Vector2 walkVelocity =
                        oldVelocity + acceleration * Time.fixedDeltaTime *
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
                        if (walkMagnitude > speed)
                        {
                            Vector2 newVelocity =
                                walkVelocity.normalized *
                                (oldMagnitude - deceleration);
                            if (newVelocity.magnitude < speed)
                                newVelocity = newVelocity.normalized * speed;
                            rb.velocity = newVelocity;
                        }
                        else
                        {
                            walkVelocity *= walkMagnitude / walkVelocity.magnitude;
                            rb.velocity = walkVelocity;
                        }
                    }

                    walkMagnitude = Mathf.Min(speed, walkMagnitude);
                    walkVelocity *= walkMagnitude / walkVelocity.magnitude;

                    rb.velocity = walkVelocity;
                }
            }
        }
    }
}
