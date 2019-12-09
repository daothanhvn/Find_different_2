﻿using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace IceFoxStudio
{
    

public class AdsManager : MonoBehaviour
{
    #region singleton

    static AdsManager _instance;

    public static AdsManager singleton
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    List<IAdsManager> adsManagers;

    private void Start()
    {
        adsManagers = new List<IAdsManager>();
        //adsManagers.Add(new UnityAdsManager());           
        adsManagers.Add(new AdmobManager());

        adsManagers.ForEach(ad =>
        {
            ad.Init();
            ad.conditionShowBanner = () =>
                GameData.Singleton.CurrentLevelPlay.Value > 2 && !GameData.Singleton.NoAds.Value;
            ad.handlerCloseAd = HandlerCloseAds;
            ad.handlerShowAd = HandlerShowAds;
        });

        GameData.Singleton.NoAds.TakeUntilDestroy(gameObject).Subscribe(value =>
        {
            if(value)
                HideBanner();
            else
            {
                ShowBanner();
            }
        });
    }

    public void ShowBanner()
    {
        adsManagers?.FirstOrDefault(ad => ad.IsReadyBanner())?.ShowBanner();
    }

    public void HideBanner()
    {
        adsManagers?.FirstOrDefault(ad => ad.IsReadyBanner())?.HiderBanner();
    }

    public void ShowInterstitial(Action cbInterstitial = null)
    {
        if (IsReadyInterstitial())
        {
            adsManagers?.FirstOrDefault(ad => ad.IsReadyIntersitial())?.ShowIntersitial(cbInterstitial);
        }
        else
        {
            cbInterstitial?.Invoke();
        }
    }

    public void ShowRewardedVideo(System.Action<bool> cbVideo = null)
    {
        var _ad = adsManagers?.FirstOrDefault(ad => ad.IsReadyVideo());

        if (_ad == null)
        {
            MessageBroker.Default.Publish(new ShowNotifyTxtMessage(){content = "Internet is not available"});
        }
        
        _ad?.ShowVideoReward(cbVideo);
    }

    void HandlerShowAds()
    {
        Time.timeScale = 0;
    }

    void HandlerCloseAds()
    {
        Time.timeScale = 1;
    }

    public bool IsReadyRewardVideo()
    {
        return adsManagers?.FirstOrDefault(ad => ad.IsReadyVideo()) != null;
    }

    public bool IsReadyInterstitial()
    {
        return adsManagers?.FirstOrDefault(ad => ad.IsReadyIntersitial()) != null && !GameData.Singleton.NoAds.Value;
    }

    System.Action cbVideoReward;

    [ContextMenu("TestShow Ads")]
    public void TestShowAds()
    {
        ShowRewardedVideo(complete => Debug.Log("AAAA"));
    }
}}
