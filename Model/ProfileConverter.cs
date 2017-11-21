using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    class ProfileConverter
	// ProfileConverter - Contains functions for converting between flow and volume and flow to position and velocity.
	//					  Needs parameters related to the mechanical construction, syringe diameter.
    {
        double SyringeDiameter { get; set; }

        public ProfileConverter()
        {
            SyringeDiameter = 10; //TODO unit?
        }

        public List<Double> FlowToPosition(List<Double> times, List<Double> flows)
        {
            return new List<Double>();
        }

        public List<Double> FlowToVelocity(List<Double> flows)
        {
            return new List<Double>();
        }

        public List<Double> PositionToVolume(List<Double> positions)
        {
            return new List<Double>();
        }

        public List<Double> VelocityToFlow(List<Double> positions)
        {
            return new List<Double>();
        }
    }
}
