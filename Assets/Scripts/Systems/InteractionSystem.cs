using Assets.Scripts.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Systems
{
    internal class InteractionSystem : MonoBehaviour
    {
        private GameObject selected;
        private GameObject hovered;
        private GameObject hoveredSprite;
        private GameObject hoveredUIElement;

        private List<IInteractionHandler> handlers = new();

        private void Start()
        {
            handlers = new();
            foreach (var handler in FindObjectsOfType<MonoBehaviour>()
                .OfType<IInteractionHandler>())
            {
                handlers.Add(handler);
            }
        }
        private void Update()
        {
            hoveredSprite = GetHoveredSprite();
            hoveredUIElement = GetHoveredUIElement();
            GameObject newHover = hoveredUIElement != null ?
                hoveredUIElement : hoveredSprite;
            GameObject newSelect = selected;
            if (Input.GetMouseButtonDown(0))
                newSelect = newHover;

            bool trigger = newSelect != null &&
                newSelect.GetComponent<Interactable>().Trigger;

            if (newSelect != selected)
            {
                Select(newSelect, trigger);
                Unselect(selected);
            }
            if (newHover != hovered)
            {
                Hover(newHover);
                Unhover(hovered);
            }

            selected = trigger ? null : newSelect;
            hovered = newHover;
        }
        private void Hover(GameObject go)
        {
            Tooltip.TooltipData data = null;
            foreach (var handler in handlers)
            {
                Tooltip.TooltipData newData = handler.Hover(go);
                if (data == null)
                    data = newData;
            }
            TooltipSystem.Instance.ShowTooltip(data);
            Interact(go, obj => obj.Hovered = true);
        }
        private void Unhover(GameObject go)
            => Interact(go, obj => obj.Hovered = false);
        private void Select(GameObject go, bool trigger)
        {
            foreach (var handler in handlers)
                handler.Select(go);
            if (!trigger)
                Interact(go, obj => obj.Selected = true);
        }
        private void Unselect(GameObject go)
            => Interact(go, obj => obj.Selected = false);
        private void Interact(GameObject go, Action<Interactable> action)
        {
            if (go == null) return;
            Interactable interactable = go.GetComponent<Interactable>();
            if (interactable == null)
            {
                Debug.LogWarning($"Tried interacting with {go} -> " +
                    $"it did not have Interactable component");
                return;
            }
            action.Invoke(interactable);

            bool hover = interactable.Hovered;
            bool select = interactable.Selected;
            if (interactable.Animator != null)
            {
                interactable.Animator.SetBool("hover", hover);
                interactable.Animator.SetBool("select", select);
            } else
            {  // Use SpriteRenderer/Image directly
                Color color = select ? Color.cyan : (hover ? Color.yellow : Color.white);
                if (interactable.Image != null)
                    interactable.Image.color = color;
                else if (interactable.SpriteRenderer != null)
                    interactable.SpriteRenderer.color = color;
                else
                    Debug.LogWarning($"Tried interacting with {go} -> " +
                        $"it does not have Animator, Image nor SpriteRenderer assigned");
            }
        }
        private bool CanInteract(GameObject go)
        {
            if (go == null) return true;
            Interactable interactable = go.GetComponent<Interactable>();
            if (interactable == null) return false;
            if (!interactable.enabled) return false;
            return true;
        }
        private GameObject GetHoveredSprite()
        {
            RaycastHit2D hitInfo = Physics2D.Raycast
                (Camera.main.ScreenToWorldPoint(Input.mousePosition),
                Vector2.zero, 1000f, 1 << LayerSystem.Instance.SpriteUILayer);
            GameObject go = null;
            if (hitInfo.collider != null)
                go = hitInfo.collider.gameObject;
            if (!CanInteract(go))
                return null;
            return go;
        }
        private GameObject GetHoveredUIElement()
        {
            PointerEventData pointerEventData = new(EventSystem.current);
            pointerEventData.position = Input.mousePosition;

            List<RaycastResult> raycastResultList = new();
            EventSystem.current.RaycastAll
                (pointerEventData, raycastResultList);
            foreach (var result in raycastResultList)
            {
                GameObject go = result.gameObject;
                if (CanInteract(go))
                    return go;
            }
            return null;
        }
    }
    public interface IInteractionHandler
    {
        Tooltip.TooltipData Hover(GameObject go);
        void Select(GameObject go);
    }
}
