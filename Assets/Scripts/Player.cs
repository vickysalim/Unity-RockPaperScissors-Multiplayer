using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Player : MonoBehaviour
{
    public Attack? AttackValue
    {
        get => chosenCard == null ? null : chosenCard.AttackValue;
    }

    public Card chosenCard;

    [SerializeField] HealthBar healthBar;
    [SerializeField] TMP_Text healthText;

    [SerializeField] GameObject nameBar;
    Color originalBarColor;
    [SerializeField] TMP_Text nameText;
    public TMP_Text Name { get => nameText; }
    
    public float Health;
    public float MaxHealth;
    
    public AudioSource audioSource;
    public AudioClip damageClip;
    
    public Transform atkPosRef;
    
    private Tweener animationTweener;

    void Start()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(PropertyNames.Room.MaxHealth, out var maxHealth))
        {
            this.MaxHealth = (float)maxHealth;
        }
        Health = MaxHealth;
        healthText.text = Health.ToString();

        originalBarColor = nameBar.GetComponent<Image>().color;
    }

    public void Reset()
    {
        if(chosenCard != null)
        {
            chosenCard.Reset();
        }

        chosenCard = null;
    }

    public void SetChosenCard(Card newCard, bool isAnimateChoose)
    {
        if(chosenCard != null)
        {
            chosenCard.Reset();
        }

        chosenCard = newCard;

        if(isAnimateChoose)
            chosenCard.transform.DOScale(chosenCard.transform.localScale * 1.1f, 0.2f);
    }

    public void ChangeHealth(float amount)
    {
        Health += amount;
        Health = Mathf.Clamp(Health, 0, MaxHealth);

        healthBar.UpdateBar(Health / MaxHealth);

        healthText.text = Health.ToString();
    }

    public void AnimateChoose()
    {
        var bar = nameBar.GetComponent<Image>();

        animationTweener = bar
            .DOColor(Color.green, 0.1f)
            .SetLoops(3, LoopType.Yoyo)
            .SetDelay(0.1f);
    }

    public void ResetAnimateChoose()
    {
        nameBar.GetComponent<Image>().color = originalBarColor;
    }

    public void AnimateAttack()
    {
        animationTweener = chosenCard.transform
            .DOMove(atkPosRef.position, 1);
    }

    public void AnimateDamage()
    {
        audioSource.PlayOneShot(damageClip);
        var image = chosenCard.GetComponent<Image>();

        animationTweener = image
            .DOColor(Color.red, 0.1f)
            .SetLoops(5, LoopType.Yoyo)
            .SetDelay(0.1f);
    }

    public void AnimateDraw()
    {
        animationTweener = chosenCard.transform
            .DOMove(chosenCard.OriginalPosition, 1)
            .SetEase(Ease.InBack)
            .SetDelay(0.1f);
    }


    public bool IsAnimating()
    {
        return animationTweener.IsActive();
    }

    public void IsClickable(bool value)
    {
        Card[] cards = GetComponentsInChildren<Card>();
        foreach(var card in cards)
        {
            card.SetClickable(value);
        }
    }
}
