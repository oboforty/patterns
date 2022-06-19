using System;

using UnityEngine;
using UnityEngine.SceneManagement;

using Assets.Scripts.Model;

namespace Assets.Scripts.Service.Setup
{
    public class GameStateExecuter
    {
        private GameStateRule[] m_Rules;
        private Scene currentScene;

        public GameStateExecuter(GameStateRule[] rules)
        {
            this.m_Rules = rules;
        }

        public void HandleRules(ServerConfig serverConfig)
        {
            currentScene = SceneManager.GetActiveScene();


            // Go through all the rules and see if they match the current state
            foreach (GameStateRule rule in m_Rules)
            {
                if (ValidateRule(rule, serverConfig))
                    ExecuteRule(rule);
            }
        }

        private bool ValidateRule(GameStateRule rule, ServerConfig serverConfig)
        {
            // @TODO: later
            if (!rule.IsInGameScene.Equals(currentScene.path.Contains("development"))) 
                return false;
            if (!rule.IsLocal.Equals(serverConfig.Url.Contains("localhost")))
                return false;


            Debug.Log("TODO: ValidateRules");
            //if (!rule.LoggedIn.Equals()) return false;
            //if (!rule.InGame.Equals()) return false;
            //if (!rule.GameInfoLoaded.Equals()) return false;
            //if (!rule.GameDataLoaded.Equals()) return false;
            //if (!rule.IsFixture.Equals()) return false;
            //if (!rule.IsLocal.Equals()) return false;

            return true;
        }

        private void ExecuteRule(GameStateRule rule)
        {
            if (RuleResponse.RedirectTo == rule.action)
            {
                if (rule.param != currentScene.name)
                    SceneManager.LoadScene(rule.param);
            }
            else if (RuleResponse.InjectAllOf == rule.action)
            {
                // @TODO: inject game data
            }
            else if (RuleResponse.Log == rule.action)
            {
                Debug.Log(rule.param);
            }
            else if (RuleResponse.LogError == rule.action)
            {
                Debug.LogError(rule.param);
            }
        }
    }

    [Serializable]
    public enum RuleResponse
    {
        RedirectTo,
        InjectAllOf,
        Log,
        LogError
    }

    [Serializable]
    public struct Boool
    {
        public bool enabled;
        public bool value;

        public override bool Equals(object obj)
        {
            if (obj is bool)
            {
                bool b = (bool)obj;

                return this.enabled ? b == value : true;
            }
            else if (obj is Boool)
            {
                Boool that = (Boool) obj;

                return that.enabled && this.enabled ? that.value == this.value : true;
            }

            return false;
        }
    }

    [Serializable]
    public class GameStateRule
    {
        [Header("IF:")]
        public Boool IsInGameScene;

        public Boool LoggedIn;
        public Boool InGame;

        public Boool GameInfoLoaded;
        public Boool GameDataLoaded;

        public Boool IsFixture;
        public Boool IsLocal;

        [Header("THEN:")]
        public RuleResponse action;
        public string param;

        public bool WasTriggered;
    }
}