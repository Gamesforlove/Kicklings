using System;
using UnityEditor.PackageManager;
using UnityEngine;

public class GameGenerator : MonoBehaviour
{
    public int gameMode;
    
    public GameObject[] Team1Buttons;
    public GameObject[] Team2Buttons;
    
    public Transform[] Team1Positions;
    public Transform[] Team2Positions;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        //testing
        //PlayerPrefs.SetInt("gamemode", -2);
        PlayerPrefs.SetInt("gamemode", 1);

        /* Retrieve Game Mode */
        int gameMode = PlayerPrefs.GetInt("gamemode");

        if (gameMode == 0){
            Debug.LogError("gameMode is 0!");
        }

        /* Indices for buttons */
        int c_team1button = 0;
        int c_team2button = 0;
        
        /* Use the gameMode to generate buttons 
         * Coop -2,  two buttons for the player team
         * Solo 1,   one button for player team
         * Duo 2,    one button for player team, one button for enemy team
         * Trio 3,   two buttons for player team, one button for enemy team
         * Quadro 4, two buttons for player team, two buttons for enemy team
         */
        while (gameMode > 0 || gameMode < 0){
        //while (gameMode > 0){ // this loop in particular does not account for coop
            if (gameMode % 2 == 0){
                //initialize enemy button
                Team2Buttons[c_team2button].SetActive(true);
                c_team2button++;
            }
            else{
                //initialize ally button
                Team1Buttons[c_team1button].SetActive(true);
                c_team1button++;
            }

            if (gameMode < 0){ gameMode++; }
            else             { gameMode--; }

            //gameMode--;
        }
        
        /*//extra loop accounting for coop, inefficient
        while (gameMode < 0){
            Team1Buttons[c_team1button].SetActive(true);
            c_team1button++;
            gameMode++;
        }*/
        
        
        
        
        /* Retrieve the team sprites */
        //GameObject playerPrefab = 
        //GameObject enemyPrefab = 

        /* Instantiate the sprites to their team positions */
        Transform p1 = null;
        Transform p2 = null;
        //Team1Positions is the player team
        for (int i = 0; i < Team1Positions.Length; i++)
        {
            p1 = Team1Positions[i];
            p2 = Team2Positions[i];
            
            //instantiate player
            //instantiate enemy

            //player.transform = p1;
            //enemy.transform = p2;

            /* Assign Players to their Buttons */
            //Team1Buttons[i].script.assignedPlayer = player
            
            // team2 buttons not instantiated in some modes, this may
            // cause issues
            //Team2Buttons[i].script.assignedPlayer = enemy
        }
    }
}
