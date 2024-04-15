using System;
using System.Collections.Generic;
using Mine.Core.Scripts.Framework.Extensions_Folder;
using UniRx;
using UnityEngine;

namespace Mine.Core.Scripts.Gameplay
{
    public class TestRxOperators : MonoBehaviour
    {
        private enum Character
        {
            A,B,C
        }
        
        private CompositeDisposable _compositeDisposable;
        private readonly ReactiveProperty<bool> _hasAvailablePerkSlot = new(false);
        private readonly ReactiveProperty<bool> _hasAvailablePerk = new(false);
        
        //2
        private readonly Dictionary<Character, ReactiveProperty<bool>> _characterNotificationDict = new();
        private readonly ReactiveProperty<Character> _selectedCharacter = new(default);

        private void OnEnable()
        {
            Observable.EveryUpdate().Subscribe(_ =>
            {
                if (Input.GetKeyDown(KeyCode.I))
                {
                    //Init();
                    //Init2();
                    //Init3();
                    //Init4();
                    Init5();
                }
            }).AddTo(gameObject);
        }

        private void Init()
        {
            _compositeDisposable = new CompositeDisposable();

            _hasAvailablePerkSlot.CombineLatest(_hasAvailablePerk, (hasPerk, hasSlot) => hasPerk && hasSlot)
                .Subscribe(ToggleNotification)
                .AddTo(_compositeDisposable);
            
            Debug.Log("Setting <color=cyan>hasAvailablePerkSlot</color> value to <color=cyan>true</color>");
            _hasAvailablePerkSlot.Value = true;

            Debug.Log("Setting <color=cyan>hasAvailablePerk</color> value to <color=cyan>true</color>");
            _hasAvailablePerk.Value = true;
        }

        private void Init2()
        {
            _compositeDisposable = new CompositeDisposable();
        
            _characterNotificationDict.Add(Character.A, new ReactiveProperty<bool>(false));
            _characterNotificationDict.Add(Character.B, new ReactiveProperty<bool>(false));
            _characterNotificationDict.Add(Character.C, new ReactiveProperty<bool>(false));

            _selectedCharacter.Select(character =>
                {
                    Debug.Log($"<color=green>Selected character set to {character}</color>");
                    return _characterNotificationDict[character];
                }).Do(hasNotification =>
                {
                    Debug.Log($"<color=yellow>Selected character has notifications: {hasNotification}</color>");
                })
                .Subscribe().AddTo(gameObject);
            
            Debug.Log("Setting <color=cyan>selectedCharacter</color> to <color=cyan>Character.B</color>");
            _selectedCharacter.Value = Character.B;
        
            Debug.Log("Setting <color=cyan>Character.B's notifications</color> to <color=cyan>true</color>");
            _characterNotificationDict[Character.B].Value = true;
        }

        private void Init3()
        {
            _compositeDisposable = new CompositeDisposable();
            _characterNotificationDict.Add(Character.A, new ReactiveProperty<bool>(false));
            _characterNotificationDict.Add(Character.B, new ReactiveProperty<bool>(false));
            _characterNotificationDict.Add(Character.C, new ReactiveProperty<bool>(false));
            
            _selectedCharacter
                .Select(character =>
                    {
                        Debug.Log($"<color=green>Selected character set to {character}</color>");
                        return _characterNotificationDict[character];
                    }
                )
                .Do(
                    hasNotifications =>
                    {
                        Debug.Log($"<color=yellow>Before switch emitted type is: <color=cyan>{hasNotifications.GetType()}</color></color>");
                        Debug.Log($"<color=yellow>Selected character has notifications: {hasNotifications.Value}</color>");
                    })
                .Switch()
                .Do(
                    hasNotifications =>
                    {
                        Debug.Log($"<color=orange>After switch emitted type is: <color=cyan>{hasNotifications.GetType()}</color></color>");
                        Debug.Log($"<color=orange>Selected character has notifications: {hasNotifications}</color>");
                    })
                .Subscribe()
                .AddTo(_compositeDisposable);
            
            Debug.Log("Setting <color=cyan>selectedCharacter</color> to <color=cyan>Character.B</color>");
            _selectedCharacter.Value = Character.B;
        
            Debug.Log("Setting <color=cyan>Character.B's notifications</color> to <color=cyan>true</color>");
            _characterNotificationDict[Character.B].Value = true;
        }
        private void Init4()
        {
            _compositeDisposable = new CompositeDisposable();

            _characterNotificationDict.Add(Character.A, new ReactiveProperty<bool>(false));
            _characterNotificationDict.Add(Character.B, new ReactiveProperty<bool>(false));
            _characterNotificationDict.Add(Character.C, new ReactiveProperty<bool>(false));

            _selectedCharacter
                .Select(
                    character =>
                    {
                        Debug.Log($"<color=green>Selected character set to {character}</color>");
                        return _characterNotificationDict[character];
                    }
                )
                .Switch()
                .Do(
                    hasNotifications =>
                    {
                        // Not recommended to do nested subscription
                        ToggleNotificationObservable(hasNotifications).Subscribe();
                    }
                )
                .Do(_ =>
                {
                    Debug.Log("<color=orange>After ToggleNotification done</color>");
                })
                .Subscribe()
                .AddTo(_compositeDisposable);

            Debug.Log("Setting C".ToBold().ToColor(Color.cyan));
            _selectedCharacter.Value = Character.C;
            _characterNotificationDict[Character.C].Value = true;
            /*Observable
                .ReturnUnit()
                .Delay(TimeSpan.FromSeconds(3))
                .Do(_ =>
                {
                    Debug.Log("Setting <color=cyan>selectedCharacter</color> to <color=cyan>Character.B</color>");
                    _selectedCharacter.Value = Character.B;
                })
                .Delay(TimeSpan.FromSeconds(3))
                .Do(_ =>
                {
                    Debug.Log("Setting <color=cyan>Character.B's notifications</color> to <color=cyan>true</color>");
                    _characterNotificationDict[Character.B].Value = true;
                })
                .Subscribe()
                .AddTo(_compositeDisposable);*/
        }

        private void Init5()
        {
            _compositeDisposable = new CompositeDisposable();

            _characterNotificationDict.Add(Character.A, new ReactiveProperty<bool>(false));
            _characterNotificationDict.Add(Character.B, new ReactiveProperty<bool>(false));
            _characterNotificationDict.Add(Character.C, new ReactiveProperty<bool>(false));
            
            _selectedCharacter
                .Select(
                    character =>
                    {
                        Debug.Log($"<color=green>Selected character set to {character}</color>");
                        return _characterNotificationDict[character];
                    }
                )
                .Switch()
                .SelectMany(hasNotification => { return ToggleNotificationObservable(hasNotification); })
                .Do(_ =>
                {
                    Debug.Log("<color=orange>After ToggleNotification done</color>");
                })
                .Subscribe()
                .AddTo(_compositeDisposable);
            
            Observable
                .ReturnUnit()
                .Delay(TimeSpan.FromSeconds(3))
                .Do(_ =>
                {
                    Debug.Log("Setting <color=cyan>selectedCharacter</color> to <color=cyan>Character.B</color>");
                    _selectedCharacter.Value = Character.B;
                })
                .Delay(TimeSpan.FromSeconds(3))
                .Do(_ =>
                {
                    Debug.Log("Setting <color=cyan>Character.B's notifications</color> to <color=cyan>true</color>");
                    _characterNotificationDict[Character.B].Value = true;
                })
                .Subscribe()
                .AddTo(_compositeDisposable);
        }

        private void OnDisable()
        {
            _compositeDisposable.Dispose();
            _characterNotificationDict.Clear();
        }

        private void ToggleNotification(bool active)
        {
            Debug.Log($"<color=yellow>Toggling notification to: {active}</color>");
        }
        
        private IObservable<Unit> ToggleNotificationObservable(bool active)
        {
            return Observable.ReturnUnit()
                .Delay(TimeSpan.FromSeconds(2))
                .Do(_ => { Debug.Log($"<color=yellow>Toggling notification to: {active}</color>"); });
        }
    }
}
