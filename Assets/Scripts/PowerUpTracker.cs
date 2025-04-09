using System.Collections.Generic;
using UnityEngine;

public class PowerUpTracker : MonoBehaviour
{
    public float powerUpCooldown;
    public float powerUpMaxDuration;
    public float remainingCooldown;
    public E_SnakeColor my_color;
    public Dictionary<E_PowerUp, float> powerUpToRemainingDuration;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        powerUpCooldown = GameManager.instance.powerUpsCooldown;
        powerUpMaxDuration = GameManager.instance.powerUpsDuration;
    }

    // Update is called once per frame
    void Update()
    {
        TickPowerUps();
        if (remainingCooldown > 0)
        {
            TickCooldown();
        }
    }

    public void Initialize(E_SnakeColor color)
    {
        my_color = color;
    }

    public bool IsOnCooldown()
    {
        if (remainingCooldown > 0)
        {
            return true;
        }
        return false;  
    }

    public void ActivatePowerUp(E_PowerUp powerUp)
    {
        powerUpToRemainingDuration[powerUp] = powerUpMaxDuration;
        UI_Controller.instance.AddPowerUp(my_color, powerUp, powerUpMaxDuration);
    }

    private void TickPowerUps()
    {
        foreach (var powerUp in powerUpToRemainingDuration.Keys)
        {
            powerUpToRemainingDuration[powerUp] -= Time.deltaTime;
            if (powerUpToRemainingDuration[powerUp] <= 0)
            {
                powerUpToRemainingDuration.Remove(powerUp);
                UI_Controller.instance.RemovePowerUp(my_color, powerUp);
            }
        }
    }

    private void TickCooldown()
    {
        remainingCooldown -= Time.deltaTime;
    }

    public bool HasPowerUp(E_PowerUp powerUp)
    { 
        return powerUpToRemainingDuration.ContainsKey(powerUp);
    }

}
