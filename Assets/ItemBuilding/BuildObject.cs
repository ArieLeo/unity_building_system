﻿using UnityEngine;
using System.Collections;

/*
 * All objects that can be placed on a Build Surface must have this Script(Or a Script that inherits it) as a component.
 * This will handle things like the contact points and collision checking.
 * When the object has been placed, this Script can be disabled or removed.
 * 
 * TODO: Assume that Build Points will not change often or at all.
 * Then we might bake it into an array instead of walking the hierarchy every time.
 * This might be done during build.
 * */

public class BuildObject : MonoBehaviour
{
	public bool ignoreCollisions = false; //Allow for ignoring any collisions when placing
	public GameObject buildPointsParent; //The parent object containing a list of Build Points as children
	public float sizeInHand = 0.5f; //The scale to use when the item is in our hand

	private int collisionCount = 0;
	private BuildPoint currentBuildPoint;

	public int getBuildPointCount ()
	{
		if (buildPointsParent == null)
			return 0;

		BuildPoint[] points = getBuildPoints ();
		return points.Length;
	}

	//Add a new Build point
	//Return: The index of the new point, or -1 if it failed to add
	public int addBuildPoint (BuildPoint point)
	{
		if (buildPointsParent == null)
			return -1;

		point.transform.parent = buildPointsParent.transform;

		return getBuildPointCount () - 1;
	}

	//Get the build point and index
	//Return: The build point, or null if index doesn't exist
	public BuildPoint getBuildPoint (int index)
	{
		if (buildPointsParent == null)
			return null;

		if (index < 0)
			return null;

		BuildPoint[] points = getBuildPoints ();
		if (index >= points.Length)
			return null;

		return points [index];
	}

	public BuildPoint[] getBuildPoints ()
	{
		if (buildPointsParent == null)
			return new BuildPoint[0];

		return buildPointsParent.GetComponentsInChildren<BuildPoint> ();
	}

	//Returns the current Build Point for this BuildObject
	//This is set before calling canPlace and placeObject on BuildSurface.
	public BuildPoint getCurrentBuildPoint ()
	{
		return currentBuildPoint;
	}

	public void setCurrentBuildPoint (BuildPoint point)
	{
		currentBuildPoint = point;
	}

	public bool canPlace (BuildSurface surface, Vector3 position, Quaternion rotation)
	{
		if (!ignoreCollisions && collisionCount != 0)
			return false;
		return true;
	}

	//Called whenever the object is being shown on the surface
	//This is where you will put an alpha on the rendering, and give it a color depening on if placement is valid
	//onSurface: True if currnetly showing on surface, false else
	//valid: Whether the current placement on the surface is valid
	public void onSurface (bool onSurface, bool valid)
	{
		//TODO: Write this properly, as it is it will not store previous values
		Renderer[] renders = GetComponentsInChildren<Renderer> ();
		foreach (Renderer render in renders) {
			if (onSurface) {
				if (!valid)
					render.material.color = new Color (1, 0.5f, 0.5f, 1f);
				else
					render.material.color = new Color (1, 1, 1, 1f);
			} else {
				render.material.color = new Color (1, 1, 1, 1);
			}
		}
	}

	public void OnCollisionEnter (Collision collision)
	{
		Debug.Log ("Collision +");
		collisionCount++;
	}

	public void OnCollisionExit (Collision collision)
	{
		Debug.Log ("Collision -");
		collisionCount--;
	}
}
