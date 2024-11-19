<img src="https://github.com/Joy-less/GameReadyGoap/blob/main/Assets/Icon.jpg?raw=true" width=256/>

# Game Ready Goap

[![NuGet](https://img.shields.io/nuget/v/GameReadyGoap.svg)](https://www.nuget.org/packages/GameReadyGoap)
 
An easy-to-use implementation of [GOAP](https://youtu.be/LhnlNKWh7oc) (Goal-Oriented Action Planning) to control game characters in C#.

## Features

- Simple and performant, made for game development
- Expressive with minimal boilerplate
- Get as close as possible to "best-effort" goals

## GOAP vs HTN

This library is parallel to [Game Ready Htn](https://github.com/Joy-less/GameReadyHtn), an implementation of HTN.

Goal-Oriented Action Planning and Hierarchical Task Networks are both powerful choices for controlling NPCs.
HTN defines a structured hierarchy of nested tasks that should be completed.
GOAP defines a set of goals and a set of actions that can be combined to reach those goals.

HTN is simpler, faster and more predictable because the tasks use a predefined order. It's suitable for most game enemies which only have a limited set of actions. It's an implementation of a behaviour tree.

GOAP is more flexible and powerful because the actions can be combined in very complex ways. It's suitable for agents with lots of actions and strategies to consider, such as in Real-Time Strategy games.

[This Reddit discussion](https://www.reddit.com/r/gamedev/comments/1ozugf) provides more comparisons between GOAP, HTN and behaviour trees.

## Usage

First, create an agent with initial states, goals and actions:
```cs
GoapAgent Agent = new() {
    // These describe the current state of your agent (character).
    States = new() {
        ...
    },
    // These are the states your agent is trying to achieve.
    Goals = [
        ...
    ],
    // These are the ways your agent can change their states.
    Actions = [
        ...
    ],
};
```

Then, finding a plan is easy:
```cs
Agent.FindPlan(); // or Agent.FindPlan(Goal);
```

Executing plans is also easy:
```cs
Plan.Execute(Agent, Action => {
    ...
});
```

## Example

A farmer is balancing tending to his crops with resting. He can farm to increase his crop health, which requires energy, or sleep to increase his energy.
```cs
GoapAgent Farmer = new() {
    States = new() {
        ["Energy"] = 100,
        ["CropHealth"] = 0,
    },
    Goals = [
        new GoapGoal("TendToCrops") {
            Objectives = [
                new GoapCondition() {
                    State = "CropHealth",
                    Comparison = GoapComparison.GreaterThanOrEqualTo,
                    Value = 100,
                    BestEffort = true,
                },
            ],
        },
    ],
    Actions = [
        new GoapAction("Farm") {
            Effects = [
                new GoapEffect() {
                    State = "CropHealth",
                    Operation = GoapOperation.IncreaseBy,
                    Value = 20,
                },
                new GoapEffect() {
                    State = "Energy",
                    Operation = GoapOperation.DecreaseBy,
                    Value = 30,
                },
            ],
            Requirements = [
                new GoapCondition() {
                    State = "Energy",
                    Comparison = GoapComparison.GreaterThanOrEqualTo,
                    Value = 30,
                },
            ],
        },
        new GoapAction("Sleep") {
            Effects = [
                new GoapEffect() {
                    State = "Energy",
                    Operation = GoapOperation.IncreaseBy,
                    Value = 5,
                },
            ],
        },
    ],
};
Farmer.FindPlan();
```

We get 15 actions which bring us to our goal:

| Action  | Energy | Crop Health |
| ------- | ------ | ----------- |
| -       | 100    | 0           |
| Farm    | 70     | 20          |
| Farm    | 40     | 40          |
| Farm    | 10     | 60          |
| Sleep   | 15     | 60          |
| Sleep   | 20     | 60          |
| Sleep   | 25     | 60          |
| Sleep   | 30     | 60          |
| Farm    | 0      | 80          |
| Sleep   | 5      | 80          |
| Sleep   | 10     | 80          |
| Sleep   | 15     | 80          |
| Sleep   | 20     | 80          |
| Sleep   | 25     | 80          |
| Sleep   | 30     | 80          |
| Farm    | 0      | 100         |

## Tips

#### Avoid near-impossible goals
Instead of aiming to set the player's health = 0, aim for <= 0.
Best-effort plans are slow to find because plans that reach the goal are always prioritised.

#### Avoid lots of actions
The more actions an agent has, the longer it will take to find a plan.

#### Know the limitations
GOAP requires a lot of trial and error to use successfully.
If configured poorly, it may result in unpredictable plans that are feasible but not believable.

## Special Thanks

- [F.E.A.R.](https://en.wikipedia.org/wiki/F.E.A.R.) for creating the GOAP algorithm.
- [This Is Vini](https://youtu.be/LhnlNKWh7oc) for explaining the GOAP algorithm.
- [SimpleGOAP](https://github.com/tckerr/SimpleGOAP) for guidance when implementing the action planner.
