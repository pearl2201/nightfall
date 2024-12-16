


using Pika.Base.Mathj.Geometry;
using System.Collections.Generic;
using System.Linq;


/**
 * 自由分配策略<br>
 * {@code FreeSlotAssignmentStrategy} is the simplest implementation of {@link SlotAssignmentStrategy}. It simply go through
 * each assignment in the list and assign sequential slot numbers. The number of slots is just the length of the list.
 * <p>
 * Because each member can occupy any slot this implementation does not support roles.
 * 
 * @param <T> Type of vector, either 2D or 3D, implementing the {@link Vector} interface
 * 
 * @author davebaol */
public class FreeSlotAssignmentStrategy<T> : SlotAssignmentStrategy<T> where T : Vector<T>
{


    public void updateSlotAssignments(List<SlotAssignment<T>> assignments)
    {
        // A very simple assignment algorithm: we simply go through
        // each assignment in the list and assign sequential slot numbers
        for (int i = 0; i < assignments.Count(); i++)
            assignments.ElementAt(i).slotNumber = i;
    }

    public int calculateNumberOfSlots(List<SlotAssignment<T>> assignments)
    {
        return assignments.Count();
    }


    public void removeSlotAssignment(List<SlotAssignment<T>> assignments, int index)
    {
        assignments.RemoveAt(index);
    }

}
