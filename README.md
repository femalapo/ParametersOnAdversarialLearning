Unity environment using MLAgents used to measure the effects that the number of parameters (observations) has on adversarial learning methods.

Agents with low, medium, and high amounts of observations (details below) were trained against each other in a simple shooting game, where the goal is to shoot the other agent before time runs out.

Conclusions:
1. A low amount of parameters leads to consistent, yet inaccurate results.
2. A high amount of parameters leads to accurate and sensitive results.
3. A medium amount of parameters shows the worst performance, with inaccurate results with no signs of learning.

Low Observations:
- Heading in relation to opponent
- Whether shooting is available
- Distance from opponent
- Opponent is visible

Medium Observations:
- All observations in "Low"
- Opponent bullet is visible
- Opponent bullet location if visible
- Opponent bullet’s heading in relation to self

High Observations:
- All observations in "Low" and "Medium"
- Opponent bullet location, regardless of visibility
- All opponent observations
