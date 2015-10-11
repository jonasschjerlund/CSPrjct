using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GrassMarch : MonoBehaviour
{
	//private bool printed = false;
	private bool FirstUpdate = true;
	public Texture2D GrassTexture;
	public Shader BillBoardShader; 
	public float TileSize = 4.0f;
	public int NumberOfTiles = 15;
	public float BillBoardScale = 0.2f;
	public int NumberOfBillBoards = 80;
	public Transform TargetTransform;
    private GameObject[,] MarchingTiles;
	private int GridOffsetX;
	private int GridOffsetZ;
	private	float Spacing; // Spacing from the edge of this grasstile, so that it doesn't overlap with the billboards from neighbours.
	private	float SafeZone; // Zone within Tile, that deducted Spacing from TileSize.

	private Mesh grassBillBoardMesh;
	private MeshRenderer newbbMeshRenderer;
	private MeshFilter newbbMeshFilter;
	private float GridSize;
	private float HalfGridSize;
	private int PrevCamX;
	private int PrevCamZ;
	private int CurCamX;
	private int CurCamZ;
	private int NumberOfIx;
	private float NearestSqrt;
	private float ltime;
	
	public Mesh MakeBillBoardMesh (float tx, float tz)
	{
		Mesh newbbMesh = new Mesh(); 
		float rScale;
		int lastix;
		Vector3 hNorm;
		bool skip;
    	List<Vector3> newbbNormals = new List<Vector3>(); 
    	List<Vector3> newbbVertices = new List<Vector3>(); 
		List<Vector2> newbbUvs = new List<Vector2>(); 
		List<int> newbbTriangles = new List<int>(); 
		Vector3 cposition;
		Vector3 modelposition;
		RaycastHit hit;
		lastix = 0;
		hNorm.x = 0.0f;
		hNorm.y = 1.0f;
		hNorm.z = 0.0f;
		int Rowcounter = 0;
		int ColCounter = 0;
		for (int cx = 0; cx < NearestSqrt ; cx ++)
		{
		for (int cy = 0; cy < NearestSqrt ; cy ++)
		{
			rScale = Random.Range(0.6f,1.0f) * BillBoardScale; 
			//modelposition.x = Spacing + cx * SafeZone/NearestSqrt + Random.Range(0.0f, SafeZone/NearestSqrt);
			//modelposition.z = Spacing + cy * SafeZone/NearestSqrt + Random.Range(0.0f, SafeZone/NearestSqrt);
			modelposition.x =  Random.Range(0.0f, TileSize );
			modelposition.z =  Random.Range(0.0f, TileSize );
			cposition.x = tx + modelposition.x;  
			cposition.y = 5000.0f; 
			cposition.z = tz + modelposition.z;  
			skip = false;
			if (Physics.Raycast(cposition , Vector3.down,out hit))
			{
			   cposition.y = hit.point.y;
			   hNorm = hit.normal;
			   if (hNorm.y < 0.7)
				{
					skip = true;
				}
				else
				{
					rScale = rScale * ((hNorm.y - 0.5f) / 0.4f);
				}
			}			
			if (skip == false)
			{
							// create four new vertices, triangle, normals, uv etc.
							newbbVertices.Add (new Vector3(modelposition.x, cposition.y,modelposition.z));
							newbbUvs.Add (new Vector2 (-0.5f * rScale,0.0f));
				            newbbNormals.Add (new Vector3 (hNorm.x, hNorm.y, hNorm.z));
							newbbVertices.Add (new Vector3(modelposition.x, cposition.y,modelposition.z));
							newbbUvs.Add (new Vector2 (- 0.5f * rScale,rScale));
				            newbbNormals.Add (new Vector3 (hNorm.x, hNorm.y, hNorm.z));
							newbbVertices.Add (new Vector3(modelposition.x, cposition.y,modelposition.z));
							newbbUvs.Add (new Vector2 (0.5f * rScale,rScale));
				            newbbNormals.Add (new Vector3 (hNorm.x, hNorm.y, hNorm.z));
							newbbVertices.Add (new Vector3(modelposition.x, cposition.y,modelposition.z));
							newbbUvs.Add (new Vector2 (0.5f * rScale,0.0f));	
				            newbbNormals.Add (new Vector3 (hNorm.x, hNorm.y, hNorm.z));
							
							newbbTriangles.Add (lastix + 0);
							newbbTriangles.Add (lastix + 1);
							newbbTriangles.Add (lastix + 3);
							newbbTriangles.Add (lastix + 3);
							newbbTriangles.Add (lastix + 1);
							newbbTriangles.Add (lastix + 2);
				            lastix = lastix + 4;				
			}
		}
		}
							
							newbbMesh.vertices = newbbVertices.ToArray();
							newbbMesh.normals = newbbNormals.ToArray();
							newbbMesh.uv = newbbUvs.ToArray() ;
							newbbMesh.triangles = newbbTriangles.ToArray();
							//newbbMesh.RecalculateNormals();
							newbbMesh.Optimize(); 	
		return newbbMesh;
	}
	
	
	// Use this for initialization
	void Start ()
	{
		// initialize MarchingTiles grid size.
		ltime = Time.realtimeSinceStartup;
		NearestSqrt = Mathf.FloorToInt (Mathf.Sqrt(NumberOfBillBoards));  
		Spacing = BillBoardScale / 3.0f;
		SafeZone = TileSize - Spacing; 
		MarchingTiles = new GameObject[NumberOfTiles,NumberOfTiles];
		NumberOfIx = NumberOfTiles - 1;
		GridOffsetX = 0;
		GridOffsetZ = 0;
		grassBillBoardMesh = MakeBillBoardMesh(0f,0f); 
		// initially we will have to perform a Mesh creation even for Each tile.
		for (int gx = 0; gx < NumberOfTiles ; gx ++)
		{
			for (int gz = 0; gz < NumberOfTiles ; gz ++)
			{
				MarchingTiles[gx,gz] = new GameObject(); 
				newbbMeshRenderer = MarchingTiles[gx,gz].AddComponent<MeshRenderer>() as MeshRenderer ;
				newbbMeshRenderer.material.shader = BillBoardShader;
				newbbMeshFilter = MarchingTiles[gx,gz].AddComponent<MeshFilter>() as MeshFilter ;
				newbbMeshFilter.mesh = grassBillBoardMesh;
				newbbMeshRenderer.material.mainTexture = GrassTexture;
				newbbMeshRenderer.material.SetFloat ("_FadeStart",NumberOfTiles/6 * TileSize); 
				newbbMeshRenderer.material.SetColor ("_Color",new Color(0.8f,1.0f,0.8f,1.0f));
				newbbMeshRenderer.material.SetFloat ("_FadeEnd",NumberOfTiles/2 * TileSize); 
				MarchingTiles[gx,gz].transform.SetParent (gameObject.transform); 
			}
			
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 newPos;
		int DiffX;
		int DiffZ;
		int VirtualX;
		int VirtualZ;
		Mesh tMesh;
		
			//initialize the offset Tracker variables for the camera...
			// these will be used to see how much the camera has moved over tiles. the direciton it moved, and the offset that the grid is drawn with.
			CurCamX = Mathf.FloorToInt(TargetTransform.position.x / TileSize);
			CurCamZ = Mathf.FloorToInt(TargetTransform.position.z / TileSize);

			if (FirstUpdate == true)
			{
				GridSize = TileSize * (float)NumberOfTiles; 
				HalfGridSize = GridSize / 2;
				for (int gx = 0; gx < NumberOfTiles ; gx ++)
				{
					for (int gz = 0; gz < NumberOfTiles ; gz ++)
					{
						newPos.x = TargetTransform.position.x - HalfGridSize + gx * TileSize;
						newPos.y = 0;
						newPos.z = TargetTransform.position.z - HalfGridSize + gz * TileSize;
						MarchingTiles[gx,gz].transform.position = newPos;
					    Destroy (MarchingTiles[gx,gz].GetComponent<MeshFilter>().mesh);
					    Destroy (MarchingTiles[gx,gz].GetComponent<MeshFilter>().sharedMesh);
						MarchingTiles[gx,gz].GetComponent<MeshFilter>().mesh  = MakeBillBoardMesh(newPos.x,newPos.z);
					}
				}
				GridOffsetX = 0;
				GridOffsetZ = 0;
				PrevCamX = CurCamX;
				PrevCamZ = CurCamZ;
				
				FirstUpdate = false;
			}
			if (CurCamX != PrevCamX || CurCamZ != PrevCamZ)
			{
				DiffX = CurCamX - PrevCamX;
				DiffZ = CurCamZ - PrevCamZ;
				if (DiffX >= NumberOfTiles || DiffZ >= NumberOfTiles || DiffX <= -NumberOfTiles || DiffZ <= -NumberOfTiles)
				{
					FirstUpdate = true;
				}
				else
				{
					if (DiffX > 0)
					{
		  				for (int gx = GridOffsetX; gx < GridOffsetX + DiffX; gx ++)
						{
			  				for (int gz = 0; gz < NumberOfTiles; gz ++)
		 				    {
								VirtualX = gx; 
								if (VirtualX > NumberOfIx){ VirtualX = VirtualX - NumberOfTiles;}
								newPos = MarchingTiles[VirtualX,gz].transform.position;
								newPos.x = newPos.x + GridSize;
								MarchingTiles[VirtualX,gz].transform.position = newPos;
					            Destroy (MarchingTiles[VirtualX,gz].GetComponent<MeshFilter>().mesh);
							    Destroy (MarchingTiles[VirtualX,gz].GetComponent<MeshFilter>().sharedMesh);
								MarchingTiles[VirtualX,gz].GetComponent<MeshFilter>().mesh  = MakeBillBoardMesh(newPos.x,newPos.z);
							}
						}
					}
					else
					{
						if (DiffX < 0)
						{
			  				for (int gx = GridOffsetX + (NumberOfTiles + DiffX); gx < GridOffsetX + NumberOfTiles; gx ++)
							{
				  				for (int gz = 0; gz < NumberOfTiles; gz ++)
			 				    {
									VirtualX = gx; 
									if (VirtualX > NumberOfIx){ VirtualX = VirtualX - NumberOfTiles;}
									newPos = MarchingTiles[VirtualX,gz].transform.position;
									newPos.x = newPos.x - GridSize;
									MarchingTiles[VirtualX,gz].transform.position = newPos;
					                Destroy (MarchingTiles[VirtualX,gz].GetComponent<MeshFilter>().mesh);
								    Destroy (MarchingTiles[VirtualX,gz].GetComponent<MeshFilter>().sharedMesh);
									MarchingTiles[VirtualX,gz].GetComponent<MeshFilter>().mesh  = MakeBillBoardMesh(newPos.x,newPos.z);
								}
							}
						}
					}
					if (DiffZ > 0)
					{
		  				for (int gz = GridOffsetZ; gz < GridOffsetZ + DiffZ; gz ++)
						{
			  				for (int gx = 0; gx < NumberOfTiles; gx ++)
		 				    {
								VirtualZ = gz; 
								if (VirtualZ > NumberOfIx){ VirtualZ = VirtualZ - NumberOfTiles;}
								newPos = MarchingTiles[gx,VirtualZ].transform.position;
								newPos.z = newPos.z + GridSize;
								MarchingTiles[gx,VirtualZ].transform.position = newPos;
					            Destroy (MarchingTiles[gx,VirtualZ].GetComponent<MeshFilter>().mesh);
								Destroy (MarchingTiles[gx,VirtualZ].GetComponent<MeshFilter>().sharedMesh);
								MarchingTiles[gx,VirtualZ].GetComponent<MeshFilter>().mesh  = MakeBillBoardMesh(newPos.x,newPos.z);
							}
						}
					}
					else
					{
						if (DiffZ < 0)
						{
			  				for (int gz = GridOffsetZ + (NumberOfTiles + DiffZ); gz < GridOffsetZ + NumberOfTiles; gz ++)
							{
				  				for (int gx = 0; gx < NumberOfTiles; gx ++)
			 				    {
									VirtualZ = gz; 
									if (VirtualZ > NumberOfIx){ VirtualZ = VirtualZ - NumberOfTiles;}
									newPos = MarchingTiles[gx,VirtualZ].transform.position;
									newPos.z = newPos.z - GridSize;
									MarchingTiles[gx,VirtualZ].transform.position = newPos;
					            	Destroy (MarchingTiles[gx,VirtualZ].GetComponent<MeshFilter>().mesh);
								    Destroy (MarchingTiles[gx,VirtualZ].GetComponent<MeshFilter>().sharedMesh);
									MarchingTiles[gx,VirtualZ].GetComponent<MeshFilter>().mesh  = MakeBillBoardMesh(newPos.x,newPos.z);
								}
							}
						}
					}
					GridOffsetX = GridOffsetX + DiffX;
					if (GridOffsetX < 0){ GridOffsetX = NumberOfTiles + GridOffsetX;}
					if (GridOffsetX > NumberOfIx){ GridOffsetX = GridOffsetX - NumberOfTiles;}
					GridOffsetZ = GridOffsetZ + DiffZ;
					if (GridOffsetZ < 0){ GridOffsetZ = NumberOfTiles + GridOffsetZ;}
					if (GridOffsetZ > NumberOfIx){ GridOffsetZ = GridOffsetZ - NumberOfTiles;}
				}
			}
			PrevCamX = CurCamX;
			PrevCamZ = CurCamZ;
		if (Time.realtimeSinceStartup  - ltime  > 7.0f)
		{
			Resources.UnloadUnusedAssets(); 
			ltime = Time.realtimeSinceStartup ;
		}
	}
}

