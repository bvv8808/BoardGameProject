using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class OmokBoard_script : MonoBehaviour {

    public float pointRange_mid;

    public Text textWin;
    Camera _camera;
    Stone_script stonePrefab_white, stonePrefab_black;
    MapNode[,] map;
    MapNode midNode;
    bool turn;
    GameObject button_set;

    struct MapNode {
        public Vector3 point;
        public int color;
    };

	// Use this for initialization
	void Start () {

        button_set = GameObject.Find("ButtonSet");
        button_set.SetActive(false);

        pointRange_mid = (float)0.4975;       
        
        _camera = FindObjectOfType<Camera>();
        stonePrefab_white = GameObject.Find("StonePrefab_white").GetComponent<Stone_script>();
        stonePrefab_black = GameObject.Find("StonePrefab_black").GetComponent<Stone_script>();
        turn = true;

        map = new MapNode[20, 20];
        int i = 0;
        int j;

        for (i = 0; i < 20; i++)
        {
            j = 0;

            map[i, j].point = new Vector3((float)-0.9827, (float)3.1103, (float)(-0.995 + i*0.10526));
            map[i, j].color = 0;

            for (j = 1; j < 20; j++)
            {
                map[i, j].point = map[i, j - 1].point + new Vector3((float)0.10526, 0, 0);
                map[i, j].color = 0;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.name == "Plane")
                {
                    int[] midNode_index = new int[2];
                    // hit포인트와 가장 가까운 Node를 찾는다. -> 함수처리
                    // 누구 턴인지에 따라 prefab 생성. 턴 관리는 GameManager에서 함.
                    // Instantiate함수를 호출하기 전에, 찾은 Node를 중심으로 주위에 선택 가능한 Node들을 List에 담는다.
                    // List를 매개변수로 넘겨서 List.Count를 범위로 하는 난수를 생성하고, 해당 인덱스의 Node를 반환하는 함수 작성.
                    // 해당 Node.point를 참조해야 하고, prefab 생성 후에 Node.color를 바꿔준다.
                    // 생성후 턴 convert 함수 호출.

                    midNode_index = IndexOfNode(hit.point);
                    midNode = map[midNode_index[0], midNode_index[1]];    // 중심노드 찾기 끝

                    if (map[midNode_index[0], midNode_index[1]].color == 0)                                 // 중심노드에 바둑알이 없을 경우에만 주위의 선택 가능한 노드List 생성
                    {
                        int x = 0, z = 0;
                        
                        MapNode selectedNode = RandomSelectNode(GetSelectableNodes(midNode_index));


                        //// map에서 selectedNode와 일치하는 Node의 index를 구하는 부분
                        //z = selectedNode.point.z + (float)0.995;        // 여긴
                        //x = selectedNode.point.x + (float)0.9827;       // 좌표

                        //if (z == 0)
                        //{
                        //    if (x != 0) x /= (float)0.10526;           // 여기부터 인덱스
                        //}
                        //else
                        //{
                        //    z /= (float)0.10526;

                        //    if (x != 0) x /= (float)0.10526;
                        //}

                        for (int i = 0; i<=19; i++)
                        {
                            for (int j = 0; j<=19; j++)
                            {
                                if (selectedNode.point == map[i,j].point)
                                {
                                    z = i;
                                    x = j;
                                }
                            }
                        }



                        // 턴에 따라 다른 색의 돌 Instantiate
                        if (turn)                                                   // 흰돌 turn == true
                        {
                            map[z, x].color = 1;
                            //map[midNode_index[0], midNode_index[1]].color = 1;
                            stonePrefab_white.MyInstantiate(map[z, x].point);
                        }
                        else
                        {
                            map[z, x].color = -1;
                            //map[midNode_index[0], midNode_index[1]].color = -1;
                            stonePrefab_black.MyInstantiate(map[z, x].point);
                        }

                        if ( !Is_Set(z, x, -1) ) turn = !turn;
                        else
                        {
                            if (turn) textWin.text = "White Win!";  
                            else textWin.text = "Black Win!";

                            button_set.SetActive(true);
                        }

                    }


                }
            }
        }
    }

    // 맵을 16구역으로 나누어 클릭한 좌표가 해당하는 구역 내에서만 최단거리 노드 탐색
    private int[] IndexOfNode(Vector3 point)
    {
        int[] indexOfNode = new int[2];

        float x = point.x / (float)2.7;
        float z = point.z / (float)2.7;
        //Debug.Log("x, z = " + x + ", " + z);
        if (z < -pointRange_mid)    // case 1~4
        {
            if(x < -pointRange_mid)
            {
                // 0,0
                indexOfNode = CalcDistance(0, 0, point);
            }
            else if( (x >= -pointRange_mid) & (x < 0) )
            {
                // 5,0
                indexOfNode = CalcDistance(5, 0, point);
            }
            else if( (x >= 0) & (x < pointRange_mid) )
            {
                // 10, 0
                indexOfNode = CalcDistance(10, 0, point);
            }
            else
            {
                // 15, 0
                indexOfNode = CalcDistance(15, 0, point);
            }
        }
        else if ( (z >= -pointRange_mid) & (z < 0) )    // case 5~8
        {
            if (x < -pointRange_mid)
            {
                // 5, 5
                indexOfNode = CalcDistance(0, 5, point);
            }
            else if ((x >= -pointRange_mid) & (x < 0))
            {
                // 5, 5
                indexOfNode = CalcDistance(5, 5, point);
            }
            else if ((x >= 0) & (x < pointRange_mid))
            {
                // 10, 5
                indexOfNode = CalcDistance(10, 5, point);
            }
            else
            {
                // 15, 5
                indexOfNode = CalcDistance(15, 5, point);
            }
        }
        else if ( (z >= 0) & (z < pointRange_mid) )     // case 9~12
        {
            if (x < -pointRange_mid)
            {
                // 0, 10
                indexOfNode = CalcDistance(0, 10, point);
            }
            else if ((x >= -pointRange_mid) & (x < 0))
            {
                // 5, 10
                indexOfNode = CalcDistance(5, 10, point);
            }
            else if ((x >= 0) & (x < pointRange_mid))
            {
                // 10, 10
                indexOfNode = CalcDistance(10, 10, point);
            }
            else
            {
                // 15, 10
                indexOfNode = CalcDistance(15, 10, point);
            }
        }
        else    // case 13~16
        {
            if (x < -pointRange_mid)
            {
                // 0, 15
                indexOfNode = CalcDistance(0, 15, point);
            }
            else if ((x >= -pointRange_mid) & (x < 0))
            {
                // 5, 15
                indexOfNode = CalcDistance(5, 15, point);
            }
            else if ((x >= 0) & (x < pointRange_mid))
            {
                // 10, 15
                indexOfNode = CalcDistance(10, 15, point);
            }
            else
            {
                // 15, 15
                indexOfNode = CalcDistance(15, 15, point);
            }
        }

        //Debug.Log("선택한 노드의 인덱스 : " + indexOfNode[0] + ", " + indexOfNode[1]);
        return indexOfNode;
    }

    // 섹터의 시작인덱스(x,z)와 hit.point를 넣어서 구역의 노드들과 point의 거리를 계산해서 가장 가까운 노드 탐색.
    private int[] CalcDistance(int index_x, int index_z, Vector3 point)
    {
        int[] result = new int[2];
        float minDistance = 500;
        float temp = 0;
        int countZ = 0;
        int countX = 0;
        float x = index_x, z = index_z;

        // 클릭한 point를 화면 좌표계(5:3)에서 전역 좌표계로 전환
        Vector3 clickedPoint = new Vector3(point.x / (float)2.7, (float)3.123, point.z / (float)2.7);

        result[0] = -1;
        result[0] = -1;
        
        while (countZ != 5)
        {
            countX = 0;

            while (countX != 5)
            {
                
                temp = Vector3.Distance(map[index_z + countZ, index_x + countX].point, clickedPoint);
                
                if ( minDistance > temp )
                {
                    minDistance = temp;
                    result[0] = index_z+countZ;
                    result[1] = index_x+countX;
                }

                countX++;
            }

            countZ++;
        }
        return result;
    }

    private List<MapNode> GetSelectableNodes(int[] midNode_index)   // 중심노드의 인덱스를 참조
    {
        List<MapNode> selectable_nodes = new List<MapNode>();

        Debug.Log("선택한 중심 노드 : " + midNode_index[0] + ", " + midNode_index[1]);
        int x = midNode_index[1]-1;
        int z = midNode_index[0]-1;

        //int count = 1;
        //int count2 = 1;

        //if (z==0)       
        //{
        //    if (x == 0)             // 좌측 최하단 노드
        //    {
        //        for(;z<2;z++)
        //        {
        //            for (int temp_x = x; temp_x < 2; temp_x++)
        //            {
        //                if (map[z, temp_x].color == 0) selectable_nodes.Add(map[z, temp_x]);
        //            }
                        
        //        }
        //    }

        //    else if (x == 19)       // 우측 최하단 노드
        //    {
                
        //        for (; z < 2; z++)
        //        {
        //            for (int temp_x = x; temp_x > 17; temp_x--)
        //            {                        
        //                if (map[z, temp_x].color == 0) selectable_nodes.Add(map[z, temp_x]);
        //            }
        //        }
        //    }

        //    else
        //    {
        //        for (; z < 2; z++)
        //        {
        //            count = 1;
        //            for (int temp_x = x - 1; count < 4; temp_x++)
        //            {
        //                if (map[z, temp_x].color == 0) selectable_nodes.Add(map[z, temp_x]);

        //                count++;
        //            }
        //        }
        //    }
        //}
        //else if (z==19)
        //{
        //    if (x == 0)             // 좌측 최상단 노드
        //    {
        //        for (; z > 17; z--)
        //        {
        //            for (int temp_x = x; temp_x < 2; temp_x++)
        //            {
        //                if (map[z, temp_x].color == 0) selectable_nodes.Add(map[z, temp_x]);
        //            }
        //        }
        //    }

        //    else if (x == 19)       // 우측 최상단 노드
        //    {
        //        for (; z > 17; z--)
        //        {
        //            for (int temp_x = x; temp_x > 17; temp_x--)
        //            {
        //                if (map[z, temp_x].color == 0) selectable_nodes.Add(map[z, temp_x]);
        //            }
        //        }
        //    }

        //    else
        //    {
        //        for (; z > 17; z--)
        //        {
        //            count = 1;
        //            for (int temp_x = x-1; count < 4; temp_x++)
        //            {
        //                if (map[z,x].color == 0) selectable_nodes.Add(map[z, temp_x]);

        //                count++;
        //            }
        //        }
        //    }
        //}
        //else
        //{
        //    if (x==0)
        //    {
        //        for (z -= 1; count < 4; z++)
        //        {
        //            for (int temp_x = x; temp_x < 2; temp_x++)
        //            {
        //                if (map[z, temp_x].color == 0) selectable_nodes.Add(map[z, temp_x]);
        //            }

        //            count++;
        //        }
        //    }

        //    else if (x==19)
        //    {
        //        for (z -= 1; count < 4; z++)
        //        {
        //            for (int temp_x = x; temp_x > 17; temp_x--)
        //            {
        //                if (map[z, temp_x].color == 0) selectable_nodes.Add(map[z, temp_x]);
        //            }

        //            count++;
        //        }
        //    }

        //    else
        //    {              // x, z 둘 다 양끝점이 아닐 때
        //        for (z -= 1; count < 4; z++)
        //        {
        //            count2 = 1;
        //            for (int temp_x = x-1; count2 < 4; temp_x++)
        //            {
        //                if (map[z, temp_x].color == 0) selectable_nodes.Add(map[z, temp_x]);

        //                count2++;
        //            }

        //            count++;
        //        }
        //    }
        //}

        for (int z2 = z+2; z<=z2; z++)
        {
            x = midNode_index[1] - 1;
            for (int x2 = x+2; x <= x2; x++)
            {
                if ( (x>=0) & (x<=19) & (z>=0) & (z<=19) )
                {

                    if (map[z, x].color == 0) { selectable_nodes.Add(map[z, x]); Debug.Log("추가된 노드 : " + z + ", " + x); }

                    }
            }
        }

        return selectable_nodes;
    }

    private MapNode RandomSelectNode(List<MapNode> list)
    {
        int r = Random.Range(0, list.Count);

        MapNode result = list[r];

        return result;
    }

    public bool Is_Set(int index_z, int index_x, int color)
    {
        int temp_z = index_z;
        int temp_x = index_x;
        int stack_one = 0;
        int stack_two = 0;


        // road 1 동쪽
        while (true)
        {
            if (++temp_x <= 19)
            {
                if (map[temp_z, temp_x].color == color) stack_one++;
                else break;
            }
            else break;
        }
        temp_x = index_x;
  

        // road 1 서쪽
        while (true)
        {
            if (temp_x >= 0)
            {
                if (map[temp_z, --temp_x].color == color) stack_two++;
                else break;
            }
            else break;
                
        }
        temp_x = index_x;

        if (stack_one + stack_two >= 4) return true;

        stack_one = 0;
        stack_two = 0;

        // road 2 남쪽
    
        while (true)
        {
            if (--temp_z >= 0)
            {
                if (map[temp_z, temp_x].color == color) stack_one++;
                else break;
            }
            else break;
        }
        temp_z = index_z;



        // road 2 북쪽
        while (true)
        {
            if (++temp_z <= 19)
            {
                if (map[temp_z, temp_x].color == color) stack_two++;
                else break;
            }
            else break;
        }
        temp_z = index_z;

        if (stack_one + stack_two >= 4) return true;

        stack_one = 0;
        stack_two = 0;


        // road 3 북동쪽
        while (true)
        {
            if (++temp_z <= 19 & ++temp_x <= 19)
            {
                if (map[temp_z, temp_x].color == color) stack_one++;
                else break;
            }
            else break;
        }
        temp_z = index_z;
        temp_x = index_x;


        // road 3 남서쪽
        while (true)
        {
            if (--temp_z >= 0 & --temp_x >= 0)
            {
                if (map[temp_z, temp_x].color == color) stack_two++;
                else break;
            }
            else break;
        }
        temp_z = index_z;
        temp_x = index_x;

        if (stack_one + stack_two >= 4) return true;

        stack_one = 0;
        stack_two = 0;


        // road 4 남동쪽 

        while (true)
        {
            if (--temp_z >= 0 & ++temp_x <= 19) {
                if (map[temp_z, temp_x].color == color) stack_one++;
                else break;
            }
            else break;
        }
        temp_z = index_z;
        temp_x = index_x;

        

        // road 4 북서쪽 

        while (true)
        {
            if (++temp_z <= 19 & --temp_x >= 0) {
                if (map[temp_z, temp_x].color == color) stack_two++;
                else break;
            }
            else break;
        }

        if (stack_one + stack_two >= 4) return true;




        return false;
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(0);
    }
}
