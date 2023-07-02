using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Systems
{
    using Assets.Scripts.Components;
    using Components.Towers;
    using UnityEngine;
    internal class LookSystem : QuickSystem<Look>
    {
        public void LookAt(Component component, Vector2 target)
        {
            if (!ComponentDict.ContainsKey(component.gameObject))
                return;
            Look look = ComponentDict[component.gameObject];
            LookAt(look, target);
        }
        public void LookAt(Look look, Vector2 target)
        {
            look.Target = target;
        }
        protected override void Tick(ICollection<Look> components)
        {
            foreach (var look in components)
            {
                Vector2 pos = look.transform.position;
                Vector3 currentEuler = look.transform.rotation.eulerAngles;
                float current = currentEuler.z;

                Vector2 targetDir = look.Target - pos;
                float goal = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
                goal %= 360f;
                if (goal < 0)
                    goal += 360f;

                // FOV
                Orientation orientation = look.CachedOrientation;
                if (orientation != null)
                {
                    //float angleDifference = Mathf.DeltaAngle(0f, goal);
                    //float maxAllowedAngle = orientation.FOV / 2f;
                    //goal = Mathf.Clamp(goal + angleDifference, -maxAllowedAngle, maxAllowedAngle);

                    float mid = orientation.Global;  // The direction "the middle" is pointing
                    float difference = Mathf.DeltaAngle(mid, goal);
                    float allowedAngle = orientation.FOV / 2f;
                    bool outOfFOV = Mathf.Abs(difference) > allowedAngle;
                    orientation.TargetOutOfFOV = outOfFOV;
                    if (outOfFOV)
                    {
                        float negAdjust = mid - allowedAngle;
                        float posAdjust = mid + allowedAngle;
                        float negDiff = Mathf.DeltaAngle(negAdjust, goal);
                        float posDiff = Mathf.DeltaAngle(posAdjust, goal);
                        if (Mathf.Abs(negDiff) < Mathf.Abs(posDiff))
                            goal = negAdjust;
                        else
                            goal = posAdjust;
                    }
                    orientation.Goal = goal;
                }

                float diff = Mathf.DeltaAngle(current, goal);//goal - current;
                //if (Mathf.Abs(diff) > 180f)
                //    diff = Mathf.Sign(diff) * (360f - Mathf.Abs(diff));

                float tickSpeed = 1000f;
                if (look.Object != null)
                    tickSpeed = Time.deltaTime * look.Object.TurnSpeed * 360f;

                float newAngle;
                bool aimExactly = Mathf.Abs(diff) <= tickSpeed;
                if (aimExactly)
                    newAngle = goal;
                else
                    newAngle = current + Mathf.Sign(diff) * tickSpeed;

                look.LookingDirectly = aimExactly;
                Vector3 newEuler = new Vector3(currentEuler.x, currentEuler.y, newAngle);
                look.transform.rotation = Quaternion.Euler(newEuler);
            }
        }
    }
}
