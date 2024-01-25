using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : MonoBehaviour
{
    // Start is called before the first frame update


    Vector2 direction;
    Rigidbody2D rb;
    float speed = 5;
    Animator animator;
    float look;
    public GameObject tremorPrefab;
    bool isMakingTremor = false;
    public GameObject fireBallPrefab;
    public GameObject startPrefab;
    float fireBallTime = 10;
    float countDown = 5;
    float starTime = 2;
    float starCountDown;
    int maxHealth = 30;
    int health;
    public GameObject keyPrefab;
    public GameObject deathPrefab;
    bool isMeet = false;
    public GameObject bearSound;
    public GameObject carrot;
    public GameObject bossHealthBar;


    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        health = maxHealth;
        bearSound.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {


        if (Mathf.Abs(GetDistance()) < 8)
        {
            bearSound.SetActive(true);
            isMeet = true;
        }
        if (!isMeet)
            return;
        Move();
        animator.SetFloat("direction", rb.velocity.x);
        animator.SetFloat("lookx", look);
        if (!Mathf.Approximately(rb.velocity.x, 0))
            look = rb.velocity.x;
        if(!isOnGround())
            isMakingTremor = true;
        if(isOnGround() && isMakingTremor)
        {
            MakeTremor();
            isMakingTremor = false;
        }
        if (countDown <= 0)
        {
            LaunchFireBall();
            countDown = fireBallTime;
        }
        countDown -= Time.deltaTime;
        starCountDown -= Time.deltaTime;
        if(starCountDown <= 0)
        {
           
            Instantiate(startPrefab, (Vector2)transform.position + Vector2.down * 2.5f, Quaternion.identity);
            starCountDown = starTime;
        }

    }

   
    void Move()
    {
        direction = GetDirection();
        if (Mathf.Abs(GetDistance()) < 10)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            Jump();
            return;
        }
        rb.velocity = new Vector2(direction.x * speed,rb.velocity.y) ;
    }
    

    Vector2 GetDirection()
    {
        float distance = GetDistance();
        Vector2 direction = new Vector2 (distance, 0);
        direction.Normalize();
        return direction;
    }


    float GetDistance()
    {
        return GameConfig.GetInstance().player.transform.position.x - rb.position.x; 
    }



    void Jump()
    {
        if (!isOnGround())
            return;
        rb.velocity = new Vector2(rb.velocity.x, 40);
    }


    void MakeTremor()
    {
        Sound.getInstance().PlayTremor();
        GameObject tremorObject =  Instantiate(tremorPrefab, (Vector2)transform.position + Vector2.down * 3.4f, Quaternion.identity);
        TremorActive tremorActive = tremorObject.GetComponent<TremorActive>();
        tremorActive.Active(new Vector2(look,0));
    }



    bool isOnGround()
    {
        bool mid = Physics2D.Raycast(rb.position + Vector2.left, Vector2.down, 4f, LayerMask.GetMask("Ground"));
        bool left = Physics2D.Raycast(rb.position, Vector2.down, 3f, LayerMask.GetMask("Ground"));
        bool right = Physics2D.Raycast(rb.position + Vector2.right, Vector2.down, 3f, LayerMask.GetMask("Ground"));
        return mid || left || right;
    }


    void LaunchFireBall()
    {
        
        GameObject fireBallObject3 = Instantiate(fireBallPrefab, rb.position + Vector2.down * 2f, Quaternion.identity);
        Sound.getInstance().PlayFireBall();
        GameObject[] fireBalls = new GameObject[1] { fireBallObject3 };
        foreach(GameObject fireBallObject in fireBalls)
        {
            FireBall fireBall = fireBallObject.GetComponent<FireBall>();
            fireBall.Launch(GetDirection());
        }
    }
    

    public void GetDamage(int d)
    {
        BossHealthBar healthBar = bossHealthBar.GetComponent<BossHealthBar>();
        if (!isMeet)
            return;
        health = Mathf.Clamp(health - d, 0, maxHealth);
        if (health <= 0)
            Death();
        healthBar.SetValue((float)health/maxHealth);
    }


    void Death()
    {
        Sound.getInstance().PlayBearDead();
        Destroy(gameObject);
        Instantiate(keyPrefab, (Vector2)transform.position + Vector2.down * 2.5f, Quaternion.identity);
        GameObject death = Instantiate(deathPrefab, (Vector2)transform.position + Vector2.down , Quaternion.identity);
        death.transform.Rotate(new Vector3(0, 180, 90));
    }



}
