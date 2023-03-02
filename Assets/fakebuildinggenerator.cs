using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fakebuildinggenerator : MonoBehaviour
{

	[SerializeField] GameObject[] fakeBuildingBlocks;
	[SerializeField] GameObject fakeSpidy;
	private Vector3 boundsHigh;
	private Vector3 boundsLow;
	private Vector3 blockSize;
	private int numberOfBlocksToFillScreen;

	// Start is called before the first frame update
	void Start()
    {
		boundsHigh = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
		boundsLow = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));

		blockSize = fakeBuildingBlocks[0].GetComponent<SpriteRenderer>().bounds.size;
		numberOfBlocksToFillScreen = Mathf.CeilToInt((boundsHigh.y - boundsLow.y)/blockSize.y);

		generateInitialBlocks(/*GameObject buildingsWrapper*/);
    }

	void generateInitialBlocks(/*GameObject buildingsWrapper*/)
	{
		float previousHeight = 0;

		// creating 2 more rows over what's necessary to fill the screen, to make regeneration
		for (int i = 1; i <= numberOfBlocksToFillScreen + 2; ++i) {
			// Destroy(buildingBlockLeft);

			Instantiate(Util.randomElement(fakeBuildingBlocks), new Vector3(boundsHigh.x - blockSize.x/2, previousHeight + boundsLow.y + blockSize.y / 2), Quaternion.identity);

			previousHeight += blockSize.y;
		}

				
		float intervalX = (boundsHigh.x - boundsLow.x)/2f - blockSize.x * 0.5f;
		float intervalY = (boundsHigh.y - boundsLow.y)/7;

		Vector2 playerSize = fakeSpidy.GetComponent<SpriteRenderer>().size;

		Vector3 spidyPosition = new Vector3(
			boundsLow.x + blockSize.x * 0.5f + intervalX * 2,
			boundsLow.y + intervalY + playerSize.y*0.5f + intervalY*2*1
		);

		Instantiate(fakeSpidy, spidyPosition, Quaternion.identity);
	}
}
