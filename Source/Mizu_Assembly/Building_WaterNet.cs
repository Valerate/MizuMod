﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using Verse;

namespace MizuMod
{
    public class Building_WaterNet : Building, IBuilding_WaterNet
    {
        public virtual bool HasConnector
        {
            get
            {
                return this.HasInputConnector || this.HasOutputConnector;
            }
        }

        public virtual bool HasInputConnector
        {
            get
            {
                return FlickUtility.WantsToBeOn(this) && this.InputConnectors.Count > 0;
            }
        }

        public virtual bool HasOutputConnector
        {
            get
            {
                return FlickUtility.WantsToBeOn(this) && this.OutputConnectors.Count > 0;
            }
        }

        public virtual bool IsSameConnector
        {
            get
            {
                return true;
            }
        }

        public MapComponent_WaterNetManager WaterNetManager
        {
            get
            {
                return this.Map.GetComponent<MapComponent_WaterNetManager>();
            }
        }

        public WaterNet InputWaterNet { get; set; }
        public WaterNet OutputWaterNet { get; set; }

        public virtual WaterType OutputWaterType
        {
            get
            {
                return WaterType.NoWater;
            }
        }

        //public virtual List<IntVec3> Connectors { get; private set; }
        public virtual List<IntVec3> InputConnectors { get; private set; }
        public virtual List<IntVec3> OutputConnectors { get; private set; }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            //this.Connectors = new List<IntVec3>();
            this.InputConnectors = new List<IntVec3>();
            this.OutputConnectors = new List<IntVec3>();
            this.CreateConnectors();

            this.WaterNetManager.AddThing(this);
        }

        public override void DeSpawn()
        {
            this.WaterNetManager.RemoveThing(this);

            base.DeSpawn();
        }

        public virtual void CreateConnectors()
        {
            this.InputConnectors.Clear();
            this.OutputConnectors.Clear();
            CellRect rect = this.OccupiedRect().ExpandedBy(1);

            foreach (var cell in rect.EdgeCells)
            {
                if (cell.x == rect.minX && cell.z == rect.minZ)
                {
                    continue;
                }
                if (cell.x == rect.minX && cell.z == rect.maxZ)
                {
                    continue;
                }
                if (cell.x == rect.maxX && cell.z == rect.minZ)
                {
                    continue;
                }
                if (cell.x == rect.maxX && cell.z == rect.maxZ)
                {
                    continue;
                }
                this.InputConnectors.Add(cell);
                this.OutputConnectors.Add(cell);
            }
        }

        public virtual void PrintForGrid(SectionLayer sectionLayer)
        {
            if (FlickUtility.WantsToBeOn(this))
            {
                MizuGraphics.LinkedWaterNetOverlay.Print(sectionLayer, this);
            }
        }

        public virtual CellRect OccupiedRect()
        {
            return GenAdj.OccupiedRect(this);
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetInspectString());

            if (DebugSettings.godMode)
            {
                if (stringBuilder.ToString() != string.Empty)
                {
                    stringBuilder.AppendLine();
                }
                if (this.InputWaterNet != null)
                {
                    stringBuilder.Append(string.Join(",", new string[] {
                        string.Format("InNetID({0})", this.InputWaterNet.ID),
                        string.Format("In({0})", this.InputWaterNet.LastInputWaterFlow.ToString("F0")),
                        string.Format("Out({0})", this.InputWaterNet.LastOutputWaterFlow.ToString("F0")),
                        string.Format("Stored({0})", this.InputWaterNet.StoredWaterVolume.ToString("F0")),
                        string.Format("Type({0})", this.InputWaterNet.WaterType),
                    }));
                }
                else
                {
                    stringBuilder.Append("InNet(null)");
                }
                stringBuilder.AppendLine();
                if (this.OutputWaterNet != null)
                {
                    stringBuilder.Append(string.Join(",", new string[] {
                        string.Format("OutNetID({0})", this.OutputWaterNet.ID),
                        string.Format("In({0})", this.OutputWaterNet.LastInputWaterFlow.ToString("F0")),
                        string.Format("Out({0})", this.OutputWaterNet.LastOutputWaterFlow.ToString("F0")),
                        string.Format("Stored({0})", this.OutputWaterNet.StoredWaterVolume.ToString("F0")),
                        string.Format("Type({0})", this.OutputWaterNet.WaterType),
                    }));
                }
                else
                {
                    stringBuilder.Append("OutNet(null)");
                }
            }

            return stringBuilder.ToString();
        }
    }
}
