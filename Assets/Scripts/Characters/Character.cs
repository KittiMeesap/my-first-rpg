using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;   

public enum CharState
{
    Idle,
    Walk,
    WalkToEnemy,
    Attack,
    WalkToMagicCast,
    MagicCast,
    Hit,
    Die
}

public abstract class Character : MonoBehaviour
{
    protected NavMeshAgent navAgent;

    protected Animator anim;
    public Animator Anim { get { return anim; } }

    [SerializeField]
    protected CharState state;
    public CharState State { get { return state; } }

    [SerializeField] 
    protected GameObject ringSelection;
    public GameObject RingSelection { get { return ringSelection; }  }

    [SerializeField]
    protected int curHP = 10;
    public int CurHP { get { return curHP; } }

    [SerializeField]
    protected Character curCharTarget;
    public Character CurCharTarget { get { return curCharTarget; } }

    [SerializeField]
    protected float attackRange = 2f;
    public float AttackTange { get { return attackRange; } }

    [SerializeField]
    protected int attackDamage = 3;

    [SerializeField]
    protected float attackCoolDown = 2f;
    [SerializeField]
    protected float attackTimer = 0f;

    [SerializeField]
    protected float findingRange = 20f;
    public float FindingRange { get { return findingRange; } }

    [SerializeField]
    protected List<Magic> magicSkills = new List<Magic>();
    public List<Magic> MagicSkills
    { get { return magicSkills; } set { magicSkills = value; } }

    [SerializeField]
    protected Magic curMagicCast = null;
    public Magic CurMagicCast
    { get { return curMagicCast; } set { curMagicCast = value; } }

    [SerializeField]
    protected bool isMagicMode = false;
    public bool IsMagicMode
    { get { return isMagicMode; } set { isMagicMode = value; } }

    [Header("Inventory")]

    [SerializeField]
    protected List<Item> inventoryItem;
    public List<Item> InventoryItem
    { get { return inventoryItem; } set { inventoryItem = value; } }

    [SerializeField]
    protected Item mainWeapon;
    public Item MainWeapon { get { return mainWeapon; } set { mainWeapon = value; } }

    [SerializeField]
    protected Item shield;
    public Item Shield { get { return shield; } set { shield = value; } }

    protected VFXManager vfxManager;
    protected UIManager uiManager;

    void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetState(CharState s)
    {
        state = s;

        if (state == CharState.Idle)
        {
            navAgent.isStopped = true;
            navAgent.ResetPath();
        }
    }

    public void WalkToPosition(Vector3 dest)
    {
        if (navAgent != null)
        {
            navAgent.SetDestination(dest);
            navAgent.isStopped = false;
        }

        SetState(CharState.Walk);
    }

    protected void WalkUpdate()
    {
        float distance = Vector3.Distance(transform.position, navAgent.destination);
        Debug.Log(distance);

        if (distance <= navAgent.stoppingDistance)
            SetState(CharState.Idle);
    }

    public void ToAttackCharacter(Character target)
    {
        if (curHP <= 0 || state == CharState.Die)
            return;

        curCharTarget = target;

        navAgent.SetDestination(target.transform.position);
        navAgent.isStopped = false;

        if (isMagicMode)
            SetState(CharState.WalkToMagicCast);
        else
            SetState(CharState.WalkToEnemy);
        
    }

    protected void WalkToEnemyUpdate()
    {
        if (curCharTarget == null)
        {
            SetState(CharState.Idle);
            return;
        }

        float distance = Vector3.Distance(transform.position, curCharTarget.transform.position);

        if (distance <= attackRange)
        {
            SetState(CharState.Attack);
            Attack(); //FirstAttack
        }
            
    }

    protected void Attack()
    {
        transform.LookAt(curCharTarget.transform);

        anim.SetTrigger("Attack");

        AttackLogic();
    }

    protected void AttackUpdate()
    {
        if (curCharTarget == null || curCharTarget.CurHP <= 0)
        {
            SetState(CharState.Idle);
            return;
        }

        navAgent.isStopped = true;

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCoolDown)
        {
            attackTimer = 0f;
            Attack();
        }

        float distance = Vector3.Distance(transform.position, curCharTarget.transform.position);

        if (distance > attackRange)
            SetState(CharState.WalkToEnemy);
    }

    protected virtual IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    protected virtual void Die()
    {
        navAgent.isStopped = true;
        SetState(CharState.Die);

        anim.SetTrigger("Die");

        StartCoroutine(DestroyObject());
    }

    public void ReceiveDamage(int damage)
    {
        if (curHP <= 0 || state == CharState.Die)
            return;

        curHP -= damage;
        if (curHP <= 0)
        {
            curHP = 0;
            Die();
        }

    }

    protected void AttackLogic()
    {
        Character target = curCharTarget.GetComponent<Character>();

        if (target != null)
            target.ReceiveDamage(attackDamage);
    }

    public bool IsMyEnemy(string targetTag)
    {
        string myTag = gameObject.tag;

        if ((myTag == "Hero" || myTag == "Player") && targetTag == "Enemy")
            return true;

        if (myTag == "Enemy" && (targetTag == "Hero" || targetTag == "Player"))
            return true;

        return false;
    }

    public void ToggleRingSelection(bool flag)
    {
        ringSelection.SetActive(flag);
    }

    public void charInit(VFXManager vfxM, UIManager uiM)
    {
        vfxManager = vfxM;
        uiManager = uiM;
    }

    protected void MagicCastLogic(Magic magic)
    {
        Character target = curCharTarget.GetComponent<Character>();

        if (target != null)
        {
            target.ReceiveDamage(magic.Power);
        }
    }

    private IEnumerator ShootMagicCast(Magic curMagicCast)
    {
        if (vfxManager != null)
            vfxManager.ShootMagic(curMagicCast.ShootID, 
                transform.position, curCharTarget.transform.position, 
                curMagicCast.ShootTime);

        yield return new WaitForSeconds(curMagicCast.ShootTime);

        MagicCastLogic(curMagicCast);
        isMagicMode = false;

        SetState(CharState.Idle);

        if (uiManager != null)
        {
            uiManager.IsOnCurToggleMagic(false);
        }
    }

    private IEnumerator LoadMagicCast(Magic curMagicCast)
    {
        if (vfxManager != null)
            vfxManager.LoadMagic(curMagicCast.LoadID,
                    transform.position, curMagicCast.LoadTime);

        yield return new WaitForSeconds(curMagicCast.LoadTime);

        StartCoroutine(ShootMagicCast(curMagicCast));
    }

    private void MagicCsat(Magic curMagicCast)
    {
        transform.LookAt(curCharTarget.transform);
        anim.SetTrigger("MagicAttack");

        StartCoroutine(LoadMagicCast(curMagicCast));
    }

    protected void WalkToMagicCastUpdate()
    {
        if (curCharTarget == null || curMagicCast == null)
        {
            SetState(CharState.Idle);
            return;
        }

        navAgent.SetDestination(curCharTarget.transform.position);

        float distance = Vector3.Distance(transform.position, curCharTarget.transform.position);

        if (distance <= curMagicCast.Range)
        {
            navAgent.isStopped = true;
            SetState(CharState.MagicCast);

            MagicCsat(curMagicCast);
        }
    }

}
