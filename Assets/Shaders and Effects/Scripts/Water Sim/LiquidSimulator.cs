using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidSimulator
{
    // Max and min cell liquid values
    float MaxValue = 1.0f;
    float MinValue = 0.005f;
    // Extra liquid a cell can store than the cell above it
    float MaxCompression = 0.25f;
    // Lowest and highest amount of liquids allowed to flow per iteration
    float MinFlow = 0.005f;
    float MaxFlow = 4f;
    // Adjusts flow speed (0.0f - 1.0f)
    float FlowSpeed = 1f;
    // Keep track of modifications to cell liquid values
    float[,] Diffs;


    public void Initialize(Cell[,] _cells)
    {
        Diffs = new float[_cells.GetLength(0), _cells.GetLength(1)];

       // FlowDirection myEnum = FlowDirection.Bottom;
    }

    public float CalculateVerticalFlowValue(float _remainingLiquid, Cell _destination)
    {
        float sum = _remainingLiquid + _destination.Liquid;
        float value = 0;

        if(sum <= MaxValue)
        {
            value = MaxValue;
        }
        else if(sum < 2* MaxValue + MaxCompression)
        {
            value = (MaxValue * MaxValue + sum * MaxCompression) / (MaxValue + MaxCompression);
        }
        else
        {
            value = (sum + MaxCompression) / 2f;
        }

        return value;
    }
    
    //Run one simulation step
    public void Simulate(ref Cell[,] _cells)
    {
        float flow = 0;
        
        //reset the diffs array
        for(int x = 0; x < Diffs.GetLength(0); x++)
        {
            for(int y = 0; y < Diffs.GetLength(1); y++)
            {
                Diffs[x, y] = 0;
            }
        }
        
        //Main Loop
        for(int x = 0; x < _cells.GetLength(0); x++)
        {
            for(int y = 0; y < _cells.GetLength(1); y++)
            {
                // get reference to Cell
                Cell cell = _cells[x, y];
                cell.ResetFlowDirections();
                
                //Validate cell
                if(cell.Type == CellType.Solid)
                {
                    cell.Liquid = 0;
                    continue;
                }

                if(cell.Liquid == 0)
                    continue;
                if(cell.Settled)
                    continue;
                if(cell.Liquid < MinValue)
                {
                    cell.Liquid = 0;
                    continue;
                }
                
                // Keep track of how much liquid this cell started with
                float startValue = cell.Liquid;
                float remainingValue = cell.Liquid;
                flow = 0;
                
                // flow to bottom cell
                if(cell.Bottom != null && cell.Bottom.Type == CellType.Blank)
                {
                    // Determine rate of flow
                    flow = CalculateVerticalFlowValue(cell.Liquid, cell.Bottom) - cell.Bottom.Liquid;
                    if(cell.Bottom.Liquid > 0 && flow > MinFlow)
                    {
                        flow *= FlowSpeed;
                    }
                    
                    // constrain flow
                    flow = Mathf.Max(flow, 0);
                    if(flow > Mathf.Min(MaxFlow, cell.Liquid))
                        flow = Mathf.Min(MaxFlow, cell.Liquid);
                    
                    //update temp values
                    if(flow != 0)
                    {
                        remainingValue -= flow;
                        Diffs[x, y] -= flow;
                        Diffs[x, y + 1] += flow;
                        cell.FlowDirections[(int)FlowDirection.Bottom] = true;
                        cell.Bottom.Settled = false;
                    }
                    
                }
                
                // check to make sure we still have liquid in the cell
                if(remainingValue < MinValue)
                {
                    Diffs[x, y] -= remainingValue;
                    continue;
                }
                
                // Flow to Left cell
                if(cell.Left != null && cell.Left.Type == CellType.Blank)
                {
                    // Calculate flow rate.
                    flow = (remainingValue - cell.Left.Liquid) / 4f;
                    if(flow > MinFlow)
                    {
                        flow *= FlowSpeed;
                    }

                    flow = Math.Max(flow, 0);
                    if(flow > Mathf.Min(MaxFlow, remainingValue))
                    {
                        flow = Mathf.Min(MaxFlow, remainingValue);
                    }

                    if(flow != 0)
                    {
                        remainingValue -= flow;
                        Diffs[x, y] -= flow;
                        Diffs[x - 1, y] += flow;
                        cell.FlowDirections[(int)FlowDirection.Left] = true;
                        cell.Left.Settled = false;
                    }
                }
                
                // check to make sure we still have liquid in the cell
                if(remainingValue < MinValue)
                {
                    Diffs[x, y] -= remainingValue;
                    continue;
                }
                
                // Flow to Right cell
                if(cell.Right != null && cell.Right.Type == CellType.Blank)
                {
                    // Calculate flow rate.
                    flow = (remainingValue - cell.Right.Liquid) / 3f;
                    if(flow > MinFlow)
                    {
                        flow *= FlowSpeed;
                    }

                    flow = Math.Max(flow, 0);
                    if(flow > Mathf.Min(MaxFlow, remainingValue))
                    {
                        flow = Mathf.Min(MaxFlow, remainingValue);
                    }

                    if(flow != 0)
                    {
                        remainingValue -= flow;
                        Diffs[x, y] -= flow;
                        Diffs[x + 1, y] += flow;
                        cell.FlowDirections[(int)FlowDirection.Right] = true;
                        cell.Right.Settled = false;
                    }
                }
                
                // check to make sure we still have liquid in the cell
                if(remainingValue < MinValue)
                {
                    Diffs[x, y] -= remainingValue;
                    continue;
                }
                
                
                // Flow to top cell
                if(cell.Top != null && cell.Top.Type == CellType.Blank)
                {
                    flow = remainingValue - CalculateVerticalFlowValue(remainingValue, cell.Top);
                    if(flow>MinFlow)
                    {
                        flow *= FlowSpeed;
                    }
                    
                    // constrain flow
                    flow = Mathf.Max(flow, 0);
                    if(flow > Mathf.Min(MaxFlow, remainingValue))
                    {
                        flow = Mathf.Min(MaxFlow, remainingValue);
                    }

                    if(flow != 0)
                    {
                        remainingValue -= flow;
                        Diffs[x, y] -= flow;
                        Diffs[x, y - 1] += flow;
                        cell.FlowDirections[(int)FlowDirection.Top] = true;
                        cell.Top.Settled = false;
                    }
                }
                
                // check to make sure we still have liquid in the cell
                if(remainingValue < MinValue)
                {
                    Diffs[x, y] -= remainingValue;
                    continue;
                }
                
                
                //Check if cell is settled
                if(startValue == remainingValue)
                {
                    cell.SettleCount++;
                    if(cell.SettleCount >= 10)
                    {
                        cell.ResetFlowDirections();
                        cell.Settled = true;
                    }
                }
                else
                {
                    cell.UnsettleNeighbors();
                }
                
                

            }
        }

        for(int x = 0; x < _cells.GetLength(0); x++)
        {
            for(int y = 0; y < _cells.GetLength(1); y++)
            {
                _cells[x, y].Liquid += Diffs[x, y];
                if(_cells[x,y].Liquid < MinValue)
                {
                    _cells[x, y].Liquid = 0;
                    _cells[x, y].Settled = false;
                }
            }
        }
    }
}
