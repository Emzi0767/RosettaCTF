// This file is part of RosettaCTF project.
//
// Copyright 2020 Emzi0767
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace RosettaCTF.Data
{
    /// <summary>
    /// Defines the scoring mode for the event.
    /// </summary>
    public enum CtfScoringMode
    {
        /// <summary>
        /// Defines static jeopardy-style scoring mode (i.e. all challenges have static amount of points). This 
        /// scoring mode will not account for challenge difficulty, which might lead to situations, wherein a team 
        /// that solves many easy challenges might score more than a team that solved smaller amount of more difficult 
        /// challenges, thus gaining a not completely fair score advantage.
        /// </summary>
        [EnumDisplayName("Jeopardy (static)")]
        Static = 0,

        /// <summary>
        /// Defines dynamic jeopardy-style scoring mode, where the number of points for each challenge will decay for 
        /// all participants as more teams solve the challenge. This is the most fair of the scoring modes, as it will 
        /// decay points for easy challenges, while retaining high scoring for difficult ones.
        /// </summary>
        [EnumDisplayName("Jeopardy")]
        Jeopardy = 1,

        /// <summary>
        /// Defines dynamic jeopardy-style scoring mode, where the number of points for each challenge will decay, 
        /// much like <see cref="Jeopardy"/>, however, once a team completes a challenge, their points will no longer 
        /// decay. Note that this scoring style is not considered fair, as it favours large, fast teams, and might 
        /// prevent smaller, slower teams, from scoring all the possible points.
        /// </summary>
        [EnumDisplayName("Jeopardy (non-persistent decay)")]
        FirstComeFirstServe = 2
    }
}
