using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGenerator : MonoBehaviour
{

    [Header("房间")]
    public int cellTotalNum;
    public List<Cell> cellPrefabs;
    [Header("冲突检测")]
    public float conflictCheckDistance;
    public Vector3 conflictCheckHalfBox;
    public LayerMask cellLayer;

    List<Cell> roomPrefabs; // 房间（单开口）
    List<Cell> corridorPrefabs; // 走廊（左右、上下开口）
    List<Cell> bigCellPrefabs;  // 其他大房间
    Cell[] allCells; 

    void Start()
    {
        SortCellPrefabs();
        StartCoroutine(GenerateLevel());
        // GenerateLevel();
        // PlacePlayer();
    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    /// <summary>
    /// 将房间预制体归类到不同列表
    /// </summary>
    void SortCellPrefabs()
    {
        roomPrefabs = new List<Cell>();
        corridorPrefabs = new List<Cell>();
        bigCellPrefabs = new List<Cell>();
        foreach (var item in cellPrefabs)
        {
            if (item.type == Cell.Type.Room)
                roomPrefabs.Add(item);
            else if (item.type == Cell.Type.Corridor)
                corridorPrefabs.Add(item);
            else
                bigCellPrefabs.Add(item);
        }
    }

    IEnumerator GenerateLevel()
    {
        List<Cell> cells = new List<Cell>();  // 当前已生成的且连接口未封闭的房间列表
        List<Cell> tempCells = new List<Cell>();    // 临时房间列表，用于随机当前匹配的房间预制体
        List<Joint> tempJoints = new List<Joint>(); // 临时房间连接口列表，用于随机当前房间连接口
        List<Joint.Type> unwantedJointTypes = new List<Joint.Type>();   // 不期望的房间连接口，用于避免生成可能会造成冲突的房间
        int cellNum; // 当前房间数量（已生成+即将生成）

        // 生成初始房间
        Cell cell = Utils.GetRandom<Cell>(corridorPrefabs);
        cell = Instantiate(cell, transform);
        cell.transform.position = transform.position;
        cells.Add(cell);
        cellNum = 1 + cell.GetAvailableJointsCount();   // 已生成1个+即将生成个数
        
        while (cellNum < cellTotalNum)
        {
            yield return new WaitForSeconds(.1f);
            // 随机获取现有未封闭房间，并取得其连接口
            cell = Utils.GetRandom<Cell>(cells);
            cell.GetAvailableJoints(tempJoints);
            var currentJoint = Utils.GetRandom<Joint>(tempJoints);

            // 检测当前连接口前方是否存在直接冲突，若存在则直接跳过
            // 或其前方的左、前、右是否存在冲突，若存在则存放在不期望的房间连接口列表中
            unwantedJointTypes.Clear();
            if (CheckConflict(currentJoint, unwantedJointTypes))
            {
                Debug.Log("检测到冲突，放弃当前房间");
                continue;
            }

            Cell matchingCell;  // 与当前连接口匹配的房间
            // 走廊与大房间轮流生成
            if (cell.type != Cell.Type.Corridor)
            {
                matchingCell = GenerateMatchingCell(corridorPrefabs, currentJoint, unwantedJointTypes, tempCells);
            }
            else
            {
                matchingCell = GenerateMatchingCell(bigCellPrefabs, currentJoint, unwantedJointTypes, tempCells);
            }

            // 没有生成合适的房间则跳过
            if (matchingCell == null)
            {
                if (cells.Count <= 1)
                {
                    Debug.Log("地图生成失败，检查Prefab是否满足要求");
                    // return;
                    yield break;
                }
                else
                {
                    Debug.Log("当前连接点没有合适的房间与之连接，跳过");
                    continue;
                }
            }

            cells.Add(matchingCell);
            // 若无剩余可用连接口则从列表中移除，更新当前房间数量
            if (cell.GetAvailableJointsCount() == 0)
                cells.Remove(cell);
            cellNum += matchingCell.GetAvailableJointsCount();
        }

        // 将剩余房间用单开口房间封闭
        foreach (var item in cells)
        {
            item.GetAvailableJoints(tempJoints);
            foreach (var joint in tempJoints)
            {
                yield return new WaitForSeconds(.1f);
                GenerateMatchingCell(roomPrefabs, joint, tempCells: tempCells);
            }
        }

        allCells = GetComponentsInChildren<Cell>();

        Debug.Log("地图生成完毕");
    }

    bool CheckConflict(Joint joint, List<Joint.Type> unwantedJointTypes)
    {
        // 先检测前方是否有冲突
        var center = joint.transform.position + joint.transform.forward * conflictCheckDistance;
        var cols = Physics.OverlapBox(center, conflictCheckHalfBox, Quaternion.identity, cellLayer, QueryTriggerInteraction.Collide);
        if (cols.Length > 0)
        {
            // 存在冲突则直接返回
            return true;
        }
        else
        {
            // 没有冲突，再检测相对于前方的前方、左方、右方是否存在冲突
            var distance = 3 * conflictCheckDistance;
            var forward = Physics.OverlapBox(center + joint.transform.forward * distance, conflictCheckHalfBox, Quaternion.identity, cellLayer, QueryTriggerInteraction.Collide);
            var left = Physics.OverlapBox(center - joint.transform.right * distance, conflictCheckHalfBox, Quaternion.identity, cellLayer, QueryTriggerInteraction.Collide);
            var right = Physics.OverlapBox(center + joint.transform.right * distance, conflictCheckHalfBox, Quaternion.identity, cellLayer, QueryTriggerInteraction.Collide);
            // 记录到不期望的连接类型列表
            if (forward.Length > 0)
                unwantedJointTypes.Add(joint.type);
            if (left.Length > 0)
                unwantedJointTypes.Add(joint.GetLocalLeft());
            if (right.Length > 0)
                unwantedJointTypes.Add(joint.GetLocalRight());
            return false;
        }
    }

    /// <summary>
    /// 生成与当前连接口相匹配的房间
    /// </summary>
    /// <param name="cellPrefabs">房间预制体列表</param>
    /// <param name="currentJoint">当前连接口</param>
    /// <param name="unwantedJointTypes">不期望的连接口类型</param>
    /// <param name="tempCells">临时房间列表，用于随机</param>
    /// <returns>匹配的房间</returns>
    Cell GenerateMatchingCell(List<Cell> cellPrefabs, Joint currentJoint, List<Joint.Type> unwantedJointTypes = null, List<Cell> tempCells = null)
    {
        // 获取期望匹配的连接口类型
        var expectedJointType = GetExpectedJointType(currentJoint.type);
        var matchingCell = GetMatchingCell(cellPrefabs, expectedJointType, unwantedJointTypes, tempCells);
        if (matchingCell == null)
            return null;
        matchingCell = Instantiate(matchingCell, transform);

        // 将两个房间的连接口位置重合，计算出生成房间位置
        var matchingJoint = matchingCell.GetJoint(expectedJointType);
        var distance = -matchingJoint.transform.localPosition;
        var cellPosition = currentJoint.transform.position + distance;

        matchingCell.transform.position = cellPosition;

        currentJoint.isUsed = true;
        matchingJoint.isUsed = true;

        return matchingCell;
    }

    /// <summary>
    /// 获取当前连接口匹配的连接口类型
    /// </summary>
    Joint.Type GetExpectedJointType(Joint.Type currentType)
    {
        // 上开口连接下开口、左开口连接右开口
        Joint.Type expectedType = default(Joint.Type);
        if (currentType == Joint.Type.Up)
            expectedType = Joint.Type.Down;
        else if (currentType == Joint.Type.Right)
            expectedType = Joint.Type.Left;
        else if (currentType == Joint.Type.Down)
            expectedType = Joint.Type.Up;
        else
            expectedType = Joint.Type.Right;
        return expectedType;
    }

    /// <summary>
    /// 获取与当前连接口匹配的房间
    /// </summary>
    /// <param name="cellPrefabs">房间预制体列表</param>
    /// <param name="expectedType">期望连接口类型</param>
    /// <param name="unwantedTypes">不期望的连接口类型</param>
    /// <param name="tempCells">临时房间列表，用于随机</param>
    Cell GetMatchingCell(List<Cell> cellPrefabs, Joint.Type expectedType, List<Joint.Type> unwantedTypes = null, List<Cell> tempCells = null)
    {
        if (tempCells == null) tempCells = new List<Cell>();
        tempCells.Clear();
        
        foreach (var item in cellPrefabs)
        {
            if (item.HasJoint(expectedType))
            {
                bool hasUnwanted = false;
                if (unwantedTypes != null)
                {
                    foreach (var unwantedType in unwantedTypes)
                    {
                        if (item.HasJoint(unwantedType))
                        {
                            hasUnwanted = true;
                            break;
                        }
                    }
                }
                if (!hasUnwanted)
                    tempCells.Add(item);
            }
        }
        
        if (tempCells.Count == 0)
            return null;
        return Utils.GetRandom<Cell>(tempCells);
    }


    void PlacePlayer()
    {
        List<Cell> rooms = new List<Cell>();
        foreach (var item in allCells)
        {
            if (item.type == Cell.Type.Room)
            {
                rooms.Add(item);
            }
        }
        var room = Utils.GetRandom<Cell>(rooms);
        var spawnPoint = Utils.GetRandom<Transform>(room.spawnPoints);

        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            var forward = Vector3.ProjectOnPlane(spawnPoint.transform.forward, player.transform.up);
            player.transform.forward = forward.normalized;
            player.transform.position = spawnPoint.transform.position;
        }
    }

}
