using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public CinemachineSmoothPath[] allTracks; // Assign all tracks in the Inspector
    private List<CinemachineSmoothPath> availableTracks; // For non-repeating tracks

    private void Awake()
    {
        // Initialize available tracks
        availableTracks = new List<CinemachineSmoothPath>(allTracks);
    }

    public CinemachineSmoothPath AssignTrack()
    {
        // Select a random track
        int randomIndex = Random.Range(0, availableTracks.Count);
        CinemachineSmoothPath selectedTrack = availableTracks[randomIndex];

        // Remove track from available list to avoid repetition
        availableTracks.RemoveAt(randomIndex);

        return selectedTrack;
    }
}
