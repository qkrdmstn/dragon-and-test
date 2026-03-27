using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class SkillObj_Flame : SkillObject
{
    [SerializeField] private float damagePeriod;
    [SerializeField] private float timer;
    [SerializeField] private List<Collider2D> inRangeTarget;

    private Player player;
    [SerializeField] private Collider2D collider2d;
    [SerializeField] private LineRenderer lineRender;
    [SerializeField] private float length;
    [SerializeField] private GameObject startEffect;
    [SerializeField] private GameObject endEffect;

    // component transform 
    [SerializeField] private float lineOffset;
    [SerializeField] private float startOffset;
    [SerializeField] private float endOffset;

    // component anim
    [SerializeField] private float fpsCounter;
    [SerializeField] private int animStep;
    [SerializeField] private float fps;
 
    [SerializeField] private Texture[] blockTextures;
    [SerializeField] private Texture[] lineTextures;

    public void Initialize(int _damage, Vector2 _dir, float _period)
    {
        player = FindObjectOfType<Player>();
        collider2d = GetComponentInChildren<Collider2D>();
        lineRender = GetComponentInChildren<LineRenderer>();

        base.Initialize(_damage, _dir);
        timer = 0.0f;
        damagePeriod = _period;
        SetComponentTransform();
    }

    private void FixedUpdate()
    {
        timer -= Time.deltaTime;
        if (timer < 0.0f)
        {
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(LayerMask.GetMask("Monster"));

            if (Physics2D.OverlapCollider(collider2d, filter.NoFilter(), inRangeTarget) != 0)
            {
                timer = damagePeriod;
                for (int i = 0; i < inRangeTarget.Count; i++)
                {
                    if (!inRangeTarget[i].CompareTag("Monster"))
                        continue;

                    MonsterBase monster = inRangeTarget[i].GetComponent<MonsterBase>();
                    monster.OnDamaged(damage);
                }
            }
        }
    }

    void Update()
    {
        SetComponentTransform();
        SetComponentAnim();
        transform.position = player.transform.position;
    }

    private void SetComponentTransform()
    {
        //Initial Direction Setting
        dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
        dir.Normalize();
        Vector3 dir3 = new Vector3(dir.x, dir.y, 0);
        Vector3[] pos = { player.transform.position + dir3 * lineOffset, player.transform.position + dir3 * length };
        lineRender.SetPositions(pos);

        float theta = Vector2.Angle(Vector2.right, dir);
        if (dir.y < 0)
            theta *= -1;
        startEffect.transform.rotation = Quaternion.Euler(0, 0, theta);
        startEffect.transform.position = player.transform.position + dir3 * startOffset;

        endEffect.transform.rotation = Quaternion.Euler(0, 0, theta);
        endEffect.transform.position = player.transform.position + length * dir3 * endOffset;
    }

    private void SetComponentAnim()
    {
        fpsCounter += Time.deltaTime;
        if (animStep == lineTextures.Length)
            animStep = 0;

        if(fpsCounter >= 1f / fps)
        {
            animStep++;

            lineRender.materials[0].SetTexture("_MainTex", blockTextures[animStep]);
            lineRender.materials[1].SetTexture("_MainTex", lineTextures[animStep]);

            fpsCounter = 0f;
        }
    }
}
