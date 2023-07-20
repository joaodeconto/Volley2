using UnityEngine;
using UnityEngine.UI;

public class MatchManager : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform ballSpawnPoint;
    [SerializeField] private Button restartBall;

    private Vector3 ballRandomPosition;
    // Start is called before the first frame update
    void Start()
    {
        restartBall.onClick.AddListener(RestartBall);   
    }

    public void RestartBall()
    {
       ballPrefab.GetComponent<Rigidbody>().velocity = Vector3.zero;
       ballRandomPosition = new Vector3(Random.Range(0f, 10f), 6, Random.Range(-5f, 5f));
       ballPrefab.transform.position = ballRandomPosition;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
