using UnityEngine;

namespace VoxelBeat.Environment
{
    public class ArenaManager : MonoBehaviour
    {
        [Header("Arena Dimensions")]
        public float width = 20f;
        public float depth = 20f;
        
        [Header("Collision Settings")]
        [SerializeField] private float wallHeight = 5f;
        [SerializeField] private float wallThickness = 1f;

        [Header("Visuals (Optional)")]
        [SerializeField] private Material floorMaterial;
        [SerializeField] private Material wallMaterial;
        [SerializeField] private Color editorGizmoColor = new Color(0f, 1f, 1f, 0.3f);

        private void Start()
        {
            GenerateArena();
        }

        [ContextMenu("Generate Arena Manually")]
        public void GenerateArena()
        {
            // Limpiar arena anterior si se regenera
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            CreateFloor();
            CreateInvisibleWalls();
        }

        private void CreateFloor()
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Arena_Floor";
            floor.transform.SetParent(transform, false);
            floor.transform.localPosition = Vector3.zero;
            
            // Los planos por defecto de Unity miden 10x10 unidades.
            // Para llegar a width/depth, dividimos por 10.
            floor.transform.localScale = new Vector3(width / 10f, 1f, depth / 10f);

            if (floorMaterial != null)
            {
                floor.GetComponent<Renderer>().material = floorMaterial;
            }
        }

        private void CreateInvisibleWalls()
        {
            // Norte (+Z)
            CreateWall("Wall_North", new Vector3(0, wallHeight / 2f, depth / 2f + wallThickness / 2f), new Vector3(width, wallHeight, wallThickness));
            // Sur (-Z)
            CreateWall("Wall_South", new Vector3(0, wallHeight / 2f, -depth / 2f - wallThickness / 2f), new Vector3(width, wallHeight, wallThickness));
            // Este (+X)
            CreateWall("Wall_East", new Vector3(width / 2f + wallThickness / 2f, wallHeight / 2f, 0), new Vector3(wallThickness, wallHeight, depth + wallThickness * 2));
            // Oeste (-X)
            CreateWall("Wall_West", new Vector3(-width / 2f - wallThickness / 2f, wallHeight / 2f, 0), new Vector3(wallThickness, wallHeight, depth + wallThickness * 2));
        }

        private void CreateWall(string wallName, Vector3 position, Vector3 size)
        {
            // Usamos CreatePrimitive para que tenga malla y sea visible
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = wallName;
            
            // El 'false' asegura que herede la rotación de la Arena
            wall.transform.SetParent(transform, false);
            wall.transform.localPosition = position;
            wall.transform.localScale = size;

            if (wallMaterial != null)
            {
                wall.GetComponent<Renderer>().material = wallMaterial;
            }
        }

        private void OnDrawGizmos()
        {
            // Aplicar la matriz de transformación para que los Gizmos roten con el objeto
            Gizmos.matrix = transform.localToWorldMatrix;

            // Esto dibuja una caja de color cyan en el editor para previsualizar la arena
            Gizmos.color = editorGizmoColor;
            Gizmos.DrawWireCube(new Vector3(0, wallHeight / 2f, 0), new Vector3(width, wallHeight, depth));
            
            // Dibujar el suelo con un poco de opacidad
            Gizmos.color = new Color(editorGizmoColor.r, editorGizmoColor.g, editorGizmoColor.b, 0.1f);
            Gizmos.DrawCube(Vector3.zero, new Vector3(width, 0.1f, depth));
        }
    }
}
