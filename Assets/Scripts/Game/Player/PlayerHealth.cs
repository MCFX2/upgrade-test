using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerHealth : MonoBehaviour
{

    [SerializeField] int maxHealth = 3;
    [SerializeField] int health = 3;

    //time that the player is invincible after taking damage
    [SerializeField] float invincibilityTime = 0.3f;
    //this controls the rate the player blinks while invincible
    [SerializeField] float invBlinkTime = 0.1f;
    float invTimeLeft = 0f;
    float blinkTimeLeft = 0f;

    SpriteRenderer m_sprite;
    private Light localLight;

    public void Damage(int damage)
    {
        if(damage > 0 && invTimeLeft > 0)
        {
            //ignore damage when we're still invincible
            return;
        }
        health -= damage;
        if(damage > 0)
        {
            invTimeLeft = invincibilityTime;
            OnDamage(damage);
        }
        else if(damage < 0)
        {
            if(health > maxHealth)
            {
                //update damage to be the "true" heal amount
                damage = maxHealth - health; //should be negative
                health = maxHealth;
            }
            OnHeal(-damage);
        }
    }

    void Start()
    {
        m_sprite = GetComponent<SpriteRenderer>();
        localLight = GetComponent<Light>();
    }

    void Update()
    {
        invTimeLeft -= Time.deltaTime;
        if(invTimeLeft > 0)
        { //only tick blink time during invuln time
            blinkTimeLeft -= Time.deltaTime;
            if (blinkTimeLeft < 0)
            {
                m_sprite.forceRenderingOff = !m_sprite.forceRenderingOff;
                localLight.enabled = !localLight.enabled;
                blinkTimeLeft += invBlinkTime;
            }
        }
        if (invTimeLeft < 0)
        {
            m_sprite.forceRenderingOff = false;
            localLight.enabled = true;
            invTimeLeft = 0;
        }
    }

    private void OnHeal(int intensity)
    {
        //screen effects, sound, etc here
    }

    private void OnDamage(int intensity)
    {
        //screen effects, sound, etc here

    }
}
