using System;
using System.Collections.Generic;
using System.Linq;

namespace Model
{
    public class ProfileConverter
	// ProfileConverter - Contains functions for converting between flow and volume and flow to position and velocity.
	//					  Needs parameters related to the mechanical construction, syringe diameter                    
    // Units
    // Position [cm]
    // Velocity [cm/s]
    // Flow     [ml/s]
    // Volume   [ml]
    {
        public double SyringeDiameter { get; set; }
        public double SectionArea { get; set; }

        public ProfileConverter()
        {
            SyringeDiameter = 3.4; // [cm]
            SectionArea = Math.Pow((SyringeDiameter / 2), 2) * Math.PI; // [cm^2]
        }

        public List<double> FlowToPosition(List<double> times, List<double> flows)
        {
            List<double> positions = new List<double>();
            List<double> volume = Integrate(times,flows);

            positions.Add((volume[0] / SectionArea));
            for (int i = 1; i <= volume.Count()-1; i++){
                positions.Add((volume[i] / SectionArea)+positions[i-1]);
            }
            return positions;
        }

        public List<double> FlowToVelocity(List<double> flows)
        {
            List<double> velocity = new List<double>();
            for (int i = 0; i < flows.Count(); i++){
                velocity.Add(flows[i]/SectionArea);
            }
            return velocity;
        }

        public List<double> PositionToVolume(List<double> positions)
        {
            List<double> volumes = new List<double>();
            for (int i = 0; i < positions.Count(); i++){
                volumes.Add(positions[i]*SectionArea);
            }
            return volumes;
        }

        public List<double> VelocityToFlow(List<double> velocities)
        {
            List<double> flows = new List<double>();
            for (int i = 0; i < velocities.Count(); i++){
                flows.Add(velocities[i]*SectionArea);
            }
            return flows;
        }
        // INTEGRATION
        static List<double> Integrate(List<double> x, List<double> y)
        {
            // calculate the integral of list y
            // first value is lost; length of list is one shorter
            if (x.Count() != y.Count())
            {
                throw new Exception("Input lists not of equal length");
            }

            List<double> primitive = new List<double>();
            for (int i = 0; i <= x.Count() - 1 - 1; i++)
            {
                double areaForward = y[i] * (x[i + 1] - x[i]);
                //double areaBackward = y[i + 1] * (x[i + 1] - x[i]); 
                primitive.Add(areaForward);
            }
            return primitive;
        }

        public List<Double> PositionToFlow(List<Double> positions, List<Double> times)
        {
            List<double> flows = new List<double>();
            for (int i = 1; i < positions.Count(); i++){
                Double deltaPosition = positions[i] - positions[i - 1];
                Double deltaTime = times[i] - times[i - 1];
                flows.Add((deltaPosition/deltaTime)*SectionArea);
            }

            return flows;
        }
    }
}
