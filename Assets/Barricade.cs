using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barricade : MonoBehaviour, IHPListener
{
    [System.Serializable]
    private class HPSpritePair
    {
        public float minHP;
        public Sprite sprite;
    }

    [SerializeField] private float maxHP;
    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private Animator animator;
    [SerializeField] private List<HPSpritePair> mapper;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<HP>().AddListener(this);
    }

    void OnEnable()
    {
        GetComponent<HP>().Set(maxHP);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void NewHP(float oldHP, float newHP)
    {
        if (newHP <= 0f)
        {
            GetComponent<Destroyable>().Destroy();
        } else
        {
            animator.SetTrigger("Hit");
            for (int i = mapper.Count-1; i >= 0; i--)
            {
                if (newHP >= mapper[i].minHP)
                {
                    renderer.sprite = mapper[i].sprite;
                    break;
                }
            }
        }
    }
}
