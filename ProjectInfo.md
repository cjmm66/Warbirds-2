
 
1. Game Overview 
Game Title: Warbirds2 
Genre: 
Action / Defense Shooter / Artillery Simulation 
Target Platform: Mobile (Landscape Mode) 
Target Audience: 
Players who enjoy skill-based action games, tactical shooters, and physics-based mechanics. 
Core Concept: 
Warbirds 2 is a military action game built around asymmetric combat scenarios. The player alternates between defensive survival gameplay and calculated offensive artillery gameplay. In defensive missions, the player controls a moving ground unit under continuous aerial attack. In offensive missions, the player operates artillery systems requiring precision, timing, and ballistic judgment. 
The intended experience combines reflex-driven combat with deliberate tactical decisionmaking. The player must balance accuracy, survival, and resource management under pressure. 
  
2. Gameplay Pillars 
•	Dual-mode combat: Reflex-based defense and calculation-based offense 
•	Constant pressure: Player is always under threat 
•	Skill over upgrades: Success depends on mastery, not grinding 
•	Tactical decision making under stress 
  
3. Core Gameplay Mechanics 
Warbirds2 is structured around two primary gameplay modes. 
In Defense Levels, the player controls either: 
•	A truck transporting an anti-air (AA) weapon system 
•	A ground soldier equipped with an RPG launcher Player actions include: 
•	Aiming weapons 
•	Firing projectiles 
•	Tracking moving aerial targets 
•	Managing reload timing 
•	Maintaining survival while moving Constraints and limitations: 
•	Limited ammunition or reload windows 
•	Continuous enemy air assaults 
•	Player vulnerability to direct hits 
•	Restricted aiming angles depending on unit type Win condition: 
•	Survive enemy waves or eliminate a defined number of aircraft or reach a checkpoint Loss condition: 
•	Player health reaches zero 
  
In Attack Levels, the player operates artillery systems. 
Player actions include: 
•	Adjusting firing angle 
•	Adjusting projectile velocity / power 
•	Selecting shell type 
•	Estimating distance and terrain interference 
•	Defending artillery from incoming threats Constraints and limitations: 
•	Indirect fire (targets not directly visible) 
•	Limited shells / resources 
•	Enemy retaliation 
•	Ballistic physics influence accuracy Win condition: 
•	Destroy designated enemy structures or bases Loss condition: 
•	Artillery destroyed or mission failure due to resource depletion 
  
4. Game Systems 
Gameplay System Purpose: 
Controls core player interactions and combat mechanics. 
Responsibilities: 
•	Weapon firing logic 
•	Projectile behavior 
•	Ballistic physics 
•	Hit detection 
•	Enemy attack patterns Interactions: 
•	Communicates with State Management System for win/loss evaluation 
•	Sends events to Feedback System 
  
State Management System Purpose: 
Tracks game progression and mission state. 
Responsibilities: 
•	Player health 
•	Enemy count 
•	Mission objectives 
•	Victory / defeat conditions 
•	Mode transitions (Defense ↔ Attack) Interactions: 
•	Receives gameplay events 
•	Triggers UI updates and mission flow changes 
  
Enemy Behavior System Purpose: 
Defines aircraft and attack unit logic. 
Responsibilities: 
•	Flight paths 
•	Attack frequency 
•	Target prioritization 
•	Difficulty scaling Interactions: 
•	Feeds threat data into Gameplay System 
•	Influences difficulty progression 
  
Scoring / Progression System Purpose: 
Provides player motivation and performance tracking. 
Responsibilities: 
•	Score calculation 
•	Performance bonuses 
•	Mission ratings 
•	Unlock logic Interactions: 
•	Receives combat results 
•	Updates UI / rewards 
  
Feedback System Purpose: 
Communicates game state and player performance. 
Responsibilities: 
•	Damage indicators 
•	Explosion effects 
•	Hit confirmations 
•	Audio feedback 
•	UI signals 
Interactions: 
•	Triggered by Gameplay & State systems 
  
5. Game Loop 
Player input is continuously collected. 
Gameplay systems update weapon, projectile, and enemy behaviors. 
State system evaluates survival and objective status. 
Feedback system delivers visual and audio responses. 
Loop repeats until: 
•	Victory condition met 
•	Defeat condition triggered 
This loop sustains tension and engagement. 
  
6. Controls & Input 
Designed for mobile landscape interaction. 
Defense Mode: 
•	Virtual joystick / drag aiming 
•	Fire button 
•	Reload / weapon action Attack Mode: 
•	Angle adjustment slider 
•	Power / velocity slider 
•	Shell selection button 
•	Fire command 
Controls prioritize precision with minimal UI clutter. 
  
 
7. Feedback & User Interface HUD Elements: 
•	Health indicator 
•	Ammunition / shells 
•	Crosshair / trajectory preview 
•	Objective indicators • Score / performance metrics Feedback Types: 
•	Visual hit markers 
•	Explosion effects 
•	Damage alerts 
•	Audio cues for firing, hits, threats 
  
8. Progression & Balance Difficulty progression: 
•	Increased enemy aggression 
•	More complex flight patterns 
•	Environmental complications (terrain, visibility) Player progression: 
•	Focus on skill mastery 
•	Gradual mechanic complexity 
•	No excessive stat inflation Balance philosophy: 
•	Challenge derived from mechanics, not numbers 
  
9. Technical Constraints (High-Level) 
•	Mobile hardware performance limitations 
•	Landscape orientation only 
•	Optimized projectile & physics calculations 
•	Limited AI complexity for stability 
•	Session-based gameplay (short missions) 
  
10. Design Justification 
The dual-mode structure creates cognitive contrast: 
Defense gameplay stresses reflexes and target tracking. Attack gameplay stresses calculation and prediction. 
This prevents gameplay fatigue and broadens player engagement. 
Projectile physics reinforce skill-based satisfaction. 
Constant threat pressure sustains tension. 
Minimal upgrade dependency avoids grind-heavy progression. 
The system-oriented design supports scalability, balancing, and iteration. 
 
