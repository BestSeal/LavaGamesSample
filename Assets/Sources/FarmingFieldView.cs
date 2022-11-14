using System.Collections.Generic;
using System.Linq;
using Sources.DataHolders;
using Sources.Farmer;
using Sources.Farmer.Actions;
using Sources.Signals;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Sources
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class FarmingFieldView : MonoBehaviour, IPointerClickHandler
    {
        [Header("If enabled shows cells coordinates when gizmos are enabled")]
        [SerializeField] private bool showCellCoords = true;
        [SerializeField] private int fieldGridTilesCountX = 10;
        [SerializeField] private int fieldGridTilesCountZ = 10;
        [SerializeField] private List<Transform> targetGroupPointsPrefab = new List<Transform>(4);

        [Header("Default placeable object for placement on unfilled cells")] [SerializeField]
        private List<FieldSelectableObject> defaultObjects
            = new List<FieldSelectableObject>();

        /// <summary>
        /// Bugged, keep it at 1 for now
        /// </summary>
        [Header("THE TILE SIZE IS BUGGED, USE ONLY VALUE OF 1, sorry for that C:")] [SerializeField]
        private float tileSize = 1;

        private MeshFilter _meshFilter;
        private Material _material;
        private Mesh _mesh;
        private MeshCollider _collider;
        private float _cashedTileSize;
        private List<FieldCell> _fieldCells;
        private SignalBus _signalBus;
        private FarmerActionsController _farmerActionsController;
        private DiContainer _container;

        private void Awake() => SetupField();

        [Inject]
        private void Initialize(SignalBus signalBus, FarmerActionsController farmerActionsController,
            DiContainer container)
        {
            _signalBus = signalBus;
            _farmerActionsController = farmerActionsController;
            _container = container;
        }

        /// <summary>
        /// Generates field mesh and setups shader settings
        /// </summary>
        private void SetupField()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _material = GetComponent<MeshRenderer>().sharedMaterial;
            _collider = GetComponent<MeshCollider>();
            _mesh = new Mesh();

            GenerateFieldMesh(_mesh);

            _meshFilter.mesh = _mesh;
            _material.SetFloat("_Height", fieldGridTilesCountX);
            _material.SetFloat("_Width", fieldGridTilesCountZ);
            _material.SetFloat("_TileSize", tileSize);
            transform.position = new Vector3(-fieldGridTilesCountX * tileSize / 2, 0,
                -fieldGridTilesCountZ * tileSize / 2);
            _collider.sharedMesh = _mesh;

            _fieldCells = GenerateFieldCells(fieldGridTilesCountX, fieldGridTilesCountZ, tileSize);

            targetGroupPointsPrefab[0].transform.position =
                new Vector3(fieldGridTilesCountX * tileSize / 2, 0, fieldGridTilesCountZ * tileSize / 2);
            targetGroupPointsPrefab[1].transform.position =
                new Vector3(-fieldGridTilesCountX * tileSize / 2, 0, fieldGridTilesCountZ * tileSize / 2);
            targetGroupPointsPrefab[2].transform.position =
                new Vector3(fieldGridTilesCountX * tileSize / 2, 0, -fieldGridTilesCountZ * tileSize / 2);
            targetGroupPointsPrefab[3].transform.position =
                new Vector3(-fieldGridTilesCountX * tileSize / 2, 0, -fieldGridTilesCountZ * tileSize / 2);
        }

        /// <summary>
        /// Handles click on field
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            var position = eventData.pointerPressRaycast.worldPosition;

            // not the fastest way, I guess that it is better to store cells as 2D matrix and index it with rounding click position
            var closestCell = _fieldCells.OrderBy(x => Vector3.Distance(x.CellPosition, position)).First();

            closestCell.OnCellClicked(eventData);
        }

        /// <summary>
        /// Highlight clicked cell on the field surface (visual effect only)
        /// </summary>
        private void HighlightClickedCell(FieldCell clickedCell)
        {
            _material.SetVector("_PositionOnSurface", new Vector2(-clickedCell.CellPosition.x + tileSize / 2,
                -clickedCell.CellPosition.z + tileSize / 2));
            _material.SetInt("_CellHighlightActive", 1);
        }

        private void OnCellObjectClickedEventHandler(FieldSelectableObject clickedObject, FieldCell sender)
        {
            var actions = clickedObject.GetActionsOnSelected();

            _signalBus.Fire(new FieldCellSelectedSignal(actions, sender));
            HighlightClickedCell(sender);
        }

        private void OnEmptyCellClickedEventHandler(FieldCell cell)
        {
            var actions = defaultObjects
                .Select(x => new PlaceAction(_farmerActionsController, x, cell, x.ObjectData.guiImage) as BaseAction)
                .ToList();
            _signalBus.Fire(new FieldCellSelectedSignal(actions, cell));
            HighlightClickedCell(cell);
        }

        /// <summary>
        /// Populates field with empty cells by provided field parameters
        /// (consider using it only once at the startup)
        /// </summary>
        /// <returns>List of new field cells</returns>
        private List<FieldCell> GenerateFieldCells(int width, int height, float cellSize)
        {
            var cells = new List<FieldCell>(width * height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var cell = new FieldCell(new Vector3(i - cellSize / 2 - width / 2f + 1, 0,
                        j - cellSize / 2 - height / 2f + 1), this, _container);
                    cell.ObjectClickedEvent += OnCellObjectClickedEventHandler;
                    cell.EmptyCellClickedEvent += OnEmptyCellClickedEventHandler;
                    cells.Add(cell);
                }
            }

            return cells;
        }

        /// <summary>
        /// Generating new mesh based on given size
        /// </summary>
        /// <param name="mesh">Mesh for assigning new mesh data</param>
        private void GenerateFieldMesh(Mesh mesh)
        {
            var quadsCount = fieldGridTilesCountX * fieldGridTilesCountZ;
            var vertices = new Vector3[(fieldGridTilesCountX + 1) * (fieldGridTilesCountZ + 1)];
            var trisIndices = new int[quadsCount * 6];
            var uvCoords = new Vector2[vertices.Length];

            for (int i = 0, z = 0; z <= fieldGridTilesCountZ; z++)
            {
                for (int x = 0; x <= fieldGridTilesCountX; x++, i++)
                {
                    vertices[i] = new Vector3(x * tileSize, 0, z * tileSize);
                    uvCoords[i] = new Vector2(x / (float)fieldGridTilesCountX, z / (float)fieldGridTilesCountZ);
                }
            }

            for (int i = 0, j = 0, z = 0; z < fieldGridTilesCountZ; z++, j++)
            {
                for (int x = 0; x < fieldGridTilesCountX; x++, i += 6, j++)
                {
                    trisIndices[i] = j;
                    trisIndices[i + 3] = trisIndices[i + 2] = j + 1;
                    trisIndices[i + 4] = trisIndices[i + 1] = j + fieldGridTilesCountX + 1;
                    trisIndices[i + 5] = j + fieldGridTilesCountX + 2;
                }
            }

            mesh.SetVertices(vertices);
            mesh.SetIndices(trisIndices, MeshTopology.Triangles, 0);
            mesh.SetUVs(0, uvCoords);
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            mesh.RecalculateNormals();
        }

        /// <summary>
        /// Editor field visualization
        /// </summary>
        private void OnDrawGizmos()
        {
            // recalculate field only after field data changed
            if (Mathf.Abs(_cashedTileSize - tileSize) < 0.001f
                && _mesh != null
                && _mesh.vertices.Length == (fieldGridTilesCountX + 1) * (fieldGridTilesCountZ + 1)) return;

            SetupField();

            if (_fieldCells != null && showCellCoords)
            {
                foreach (var cell in _fieldCells)
                {
                    Handles.Label(cell.CellPosition + new Vector3(0, 0.5f, 0), cell.CellPosition.ToString());
                }
            }
        }
    }
}