using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EducationalContent : MonoBehaviour
{
    [SerializeField] List<Sprite> educationContentImgList;
    [SerializeField] Transform eduContentPanel;

    public void showEducationalContent()
    {
        eduContentPanel.gameObject.SetActive(true);
        eduContentPanel.localPosition = new Vector3(0f, 0f, 0f);

        CorouselEducation.Instance.contentPanels = educationContentImgList;
        CorouselEducation.Instance.ShowContent();
    }
}
