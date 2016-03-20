using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*--------------------------------------------------------
 * CubeMap.cs
 * 
 * Version: 1.0
 * Author: Filipe
 * Created: 20/03/2016 19:16:20
 * 
 * Notes:
 * Code mostly based on: http://stackoverflow.com/questions/9929103/need-help-using-instancing-in-xna-4-0
 * for testing purpose.
 * -------------------------------------------------------*/

namespace HardwareInstancing
{
    public class CubeMap : DrawableGameComponent
    {
        #region FIELDS

        private Texture2D texture;
        private Effect effect;

        private VertexDeclaration instanceVertexDeclaration;

        private VertexBuffer instanceBuffer;
        private VertexBuffer geometryBuffer;
        private IndexBuffer indexBuffer;

        private VertexBufferBinding[] bindings;
        private CubeInfo[] instances;

        struct CubeInfo
        {
            public Vector4 World;
            public Vector2 AtlasCoordinate;
        };

        private Int32 sizeX;
        private Int32 sizeZ;

        #endregion

        #region PROPERTIES

        public Matrix View { get; set; }

        public Matrix Projection { get; set; }

        public Int32 InstanceCount
        {
            get { return this.sizeX * this.sizeZ; }
        }

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Creates a new CubeMap.
        /// </summary>
        /// <param name="game">Parent game instance.</param>
        /// <param name="sizeX">Map size on X.</param>
        /// <param name="sizeZ">Map size on Z.</param>
        public CubeMap(Game game, int sizeX, int sizeZ)
            : base(game)
        {
            this.sizeX = sizeX;
            this.sizeZ = sizeZ;
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Initialize the VertexBuffer declaration for one cube instance.
        /// </summary>
        private void InitializeInstanceVertexBuffer()
        {
            VertexElement[] _instanceStreamElements = new VertexElement[2];

            // Position
            _instanceStreamElements[0] = new VertexElement(0, VertexElementFormat.Vector4,
                        VertexElementUsage.Position, 1);

            // Texture coordinate
            _instanceStreamElements[1] = new VertexElement(sizeof(Single) * 4, VertexElementFormat.Vector2,
                    VertexElementUsage.TextureCoordinate, 1);

            this.instanceVertexDeclaration = new VertexDeclaration(_instanceStreamElements);
        }

        /// <summary>
        /// Initialize all the cube instance. (sizeX * sizeZ)
        /// </summary>
        private void InitializeInstances()
        {
            Random _randomHeight = new Random();
            this.instances = new CubeInfo[this.InstanceCount];

            // Set the position for each cube.
            for (Int32 i = 0; i < this.sizeX; ++i)
            {
                for (Int32 j = 0; j < this.sizeZ; ++j)
                {
                    this.instances[i * this.sizeX + j].World = new Vector4(i * 2, _randomHeight.Next(0, 2), j * 2, 1);
                    this.instances[i * this.sizeX + j].AtlasCoordinate = new Vector2(_randomHeight.Next(0, 2), _randomHeight.Next(0, 2));
                }
            }

            // Set the instace data to the instanceBuffer.
            this.instanceBuffer = new VertexBuffer(this.GraphicsDevice, instanceVertexDeclaration, this.InstanceCount, BufferUsage.WriteOnly);
            this.instanceBuffer.SetData(this.instances);
        }

        /// <summary>
        /// Generate the common cube geometry. (Only one cube)
        /// </summary>
        private void GenerateCommonGeometry()
        {
            VertexPositionTexture[] _vertices = new VertexPositionTexture[24];

            #region filling vertices
            _vertices[0].Position = new Vector3(-1, 1, -1);
            _vertices[0].TextureCoordinate = new Vector2(0, 0);
            _vertices[1].Position = new Vector3(1, 1, -1);
            _vertices[1].TextureCoordinate = new Vector2(1, 0);
            _vertices[2].Position = new Vector3(-1, 1, 1);
            _vertices[2].TextureCoordinate = new Vector2(0, 1);
            _vertices[3].Position = new Vector3(1, 1, 1);
            _vertices[3].TextureCoordinate = new Vector2(1, 1);

            _vertices[4].Position = new Vector3(-1, -1, 1);
            _vertices[4].TextureCoordinate = new Vector2(0, 0);
            _vertices[5].Position = new Vector3(1, -1, 1);
            _vertices[5].TextureCoordinate = new Vector2(1, 0);
            _vertices[6].Position = new Vector3(-1, -1, -1);
            _vertices[6].TextureCoordinate = new Vector2(0, 1);
            _vertices[7].Position = new Vector3(1, -1, -1);
            _vertices[7].TextureCoordinate = new Vector2(1, 1);

            _vertices[8].Position = new Vector3(-1, 1, -1);
            _vertices[8].TextureCoordinate = new Vector2(0, 0);
            _vertices[9].Position = new Vector3(-1, 1, 1);
            _vertices[9].TextureCoordinate = new Vector2(1, 0);
            _vertices[10].Position = new Vector3(-1, -1, -1);
            _vertices[10].TextureCoordinate = new Vector2(0, 1);
            _vertices[11].Position = new Vector3(-1, -1, 1);
            _vertices[11].TextureCoordinate = new Vector2(1, 1);

            _vertices[12].Position = new Vector3(-1, 1, 1);
            _vertices[12].TextureCoordinate = new Vector2(0, 0);
            _vertices[13].Position = new Vector3(1, 1, 1);
            _vertices[13].TextureCoordinate = new Vector2(1, 0);
            _vertices[14].Position = new Vector3(-1, -1, 1);
            _vertices[14].TextureCoordinate = new Vector2(0, 1);
            _vertices[15].Position = new Vector3(1, -1, 1);
            _vertices[15].TextureCoordinate = new Vector2(1, 1);

            _vertices[16].Position = new Vector3(1, 1, 1);
            _vertices[16].TextureCoordinate = new Vector2(0, 0);
            _vertices[17].Position = new Vector3(1, 1, -1);
            _vertices[17].TextureCoordinate = new Vector2(1, 0);
            _vertices[18].Position = new Vector3(1, -1, 1);
            _vertices[18].TextureCoordinate = new Vector2(0, 1);
            _vertices[19].Position = new Vector3(1, -1, -1);
            _vertices[19].TextureCoordinate = new Vector2(1, 1);

            _vertices[20].Position = new Vector3(1, 1, -1);
            _vertices[20].TextureCoordinate = new Vector2(0, 0);
            _vertices[21].Position = new Vector3(-1, 1, -1);
            _vertices[21].TextureCoordinate = new Vector2(1, 0);
            _vertices[22].Position = new Vector3(1, -1, -1);
            _vertices[22].TextureCoordinate = new Vector2(0, 1);
            _vertices[23].Position = new Vector3(-1, -1, -1);
            _vertices[23].TextureCoordinate = new Vector2(1, 1);
            #endregion

            this.geometryBuffer = new VertexBuffer(this.GraphicsDevice, VertexPositionTexture.VertexDeclaration,
                                              24, BufferUsage.WriteOnly);
            this.geometryBuffer.SetData(_vertices);

            #region filling indices

            Int32[] _indices = new Int32[36];
            _indices[0] = 0; _indices[1] = 1; _indices[2] = 2;
            _indices[3] = 1; _indices[4] = 3; _indices[5] = 2;

            _indices[6] = 4; _indices[7] = 5; _indices[8] = 6;
            _indices[9] = 5; _indices[10] = 7; _indices[11] = 6;

            _indices[12] = 8; _indices[13] = 9; _indices[14] = 10;
            _indices[15] = 9; _indices[16] = 11; _indices[17] = 10;

            _indices[18] = 12; _indices[19] = 13; _indices[20] = 14;
            _indices[21] = 13; _indices[22] = 15; _indices[23] = 14;

            _indices[24] = 16; _indices[25] = 17; _indices[26] = 18;
            _indices[27] = 17; _indices[28] = 19; _indices[29] = 18;

            _indices[30] = 20; _indices[31] = 21; _indices[32] = 22;
            _indices[33] = 21; _indices[34] = 23; _indices[35] = 22;

            #endregion

            this.indexBuffer = new IndexBuffer(this.GraphicsDevice, typeof(Int32), 36, BufferUsage.WriteOnly);
            this.indexBuffer.SetData(_indices);
        }

        #endregion

        #region OVERRIDE METHODS

        /// <summary>
        /// Initialize the CubeMap.
        /// </summary>
        public override void Initialize()
        {
            this.InitializeInstanceVertexBuffer();
            this.GenerateCommonGeometry();
            this.InitializeInstances();

            // Creates the binding between the geometry and the instances.
            this.bindings = new VertexBufferBinding[2];
            this.bindings[0] = new VertexBufferBinding(geometryBuffer);
            this.bindings[1] = new VertexBufferBinding(instanceBuffer, 0, 1);

            base.Initialize();
        }

        /// <summary>
        /// Load the CubeMap effect and texture.
        /// </summary>
        protected override void LoadContent()
        {
            this.effect = this.Game.Content.Load<Effect>("instance_effect");
            this.texture = this.Game.Content.Load<Texture2D>("stone");

            base.LoadContent();
        }

        /// <summary>
        /// Update the CubeMap logic.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// Draw the cube map using one single vertexbuffer.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            // Set the effect technique and parameters
            this.effect.CurrentTechnique = effect.Techniques["Instancing"];
            this.effect.Parameters["WorldViewProjection"].SetValue(this.View * this.Projection);
            this.effect.Parameters["cubeTexture"].SetValue(texture);

            // Set the indices in the graphics device.
            this.GraphicsDevice.Indices = indexBuffer;

            // Apply the current technique pass.
            this.effect.CurrentTechnique.Passes[0].Apply();

            // Set the vertex buffer and draw the instanced primitives.
            this.GraphicsDevice.SetVertexBuffers(bindings);
            this.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 24, 0, 12, this.InstanceCount);

            base.Draw(gameTime);
        }

        #endregion
    }
}
