using System;
using GTA.Math;
using GTA.Native;
using GTA.NaturalMotion;

namespace GTA
{
	public enum Gender
	{
		Male,
		Female
	}
	public enum DrivingStyle
	{
		Normal = 786603,
		IgnoreLights = 2883621,
		SometimesOvertakeTraffic = 5,
		Rushed = 1074528293,
		AvoidTraffic = 786468,
		AvoidTrafficExtremely = 6
	}
	public enum HelmetType : uint
	{
		RegularMotorcycleHelmet = 4096u,
		FiremanHelmet = 16384u,
		PilotHeadset = 32768u
	}
	public enum ParachuteLandingType
	{
		None = -1,
		Stumbling = 1,
		Rolling,
		Ragdoll
	}
	public enum ParachuteState
	{
		None = -1,
		FreeFalling,
		Deploying,
		Gliding,
		LandingOrFallingToDoom
	}

	public sealed class Ped : Entity
	{
		#region Fields
		Tasks _tasks;
		Euphoria _euphoria;
		WeaponCollection _weapons;
		#endregion

		public Ped(int handle) : base(handle)
		{
		}

		/// <summary>
		/// Gets or sets how much money the <see cref="Ped"/> is carrying.
		/// </summary>
		public int Money
		{
			get
			{
				return Function.Call<int>(Hash.GET_PED_MONEY, Handle);
			}
			set
			{
				Function.Call(Hash.SET_PED_MONEY, Handle, value);
			}
		}
		/// <summary>
		/// Gets the gender of the <see cref="Ped"/>.
		/// </summary>

		public Gender Gender
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_MALE, Handle) ? Gender.Male : Gender.Female;
			}
		}
		/// <summary>
		/// Gets or sets the maximum health of the <see cref="Ped"/>.
		/// </summary>
		public override int MaxHealth
		{
			get
			{
				return Function.Call<int>(Hash.GET_PED_MAX_HEALTH, Handle);
			}
			set
			{
				Function.Call(Hash.SET_PED_MAX_HEALTH, Handle, value);
			}
		}
		/// <summary>
		/// Gets or sets how much Armor the <see cref="Ped"/> is wearing.
		/// </summary>
		public int Armor
		{
			get
			{
				return Function.Call<int>(Hash.GET_PED_ARMOUR, Handle);
			}
			set
			{
				Function.Call(Hash.SET_PED_ARMOUR, Handle, value);
			}
		}
		/// <summary>
		/// Gets or sets how accurate the <see cref="Ped"/>s shooting ability is.
		/// </summary>
		/// <value>
		/// The accuracy from 0 to 100, 0 being very innacurate, 100 being perfectly accurate.
		/// </value>
		public int Accuracy
		{
			get
			{
				return Function.Call<int>(Hash.GET_PED_ACCURACY, Handle);
			}
			set
			{
				Function.Call(Hash.SET_PED_ACCURACY, Handle, value);
			}
		}

		/// <summary>
		/// Opens a list of <see cref="Tasks"/> that the <see cref="Ped"/> can carry out.
		/// </summary>
		public Tasks Task
		{
			get
			{
				if (ReferenceEquals(_tasks, null))
				{
					_tasks = new Tasks(this);
				}
				return _tasks;
			}
		}
		/// <summary>
		/// Gets the stage of the <see cref="TaskSequence"/> the <see cref="Ped"/> is currently executing.
		/// </summary>
		public int TaskSequenceProgress
		{
			get
			{
				return Function.Call<int>(Hash.GET_SEQUENCE_PROGRESS, Handle);
			}
		}

		/// <summary>
		/// Opens a list of <see cref="GTA.NaturalMotion.Euphoria"/> Helpers which can be applie to the <see cref="Ped"/>.
		/// </summary>
		public Euphoria Euphoria
		{
			get
			{
				if (ReferenceEquals(_euphoria, null))
				{
					_euphoria = new Euphoria(this);
				}
				return _euphoria;
			}
		}

		/// <summary>
		/// Gets a collection of all the <see cref="Ped"/>s <see cref="Weapon"/>s.
		/// </summary>
		public WeaponCollection Weapons
		{
			get
			{
				if (ReferenceEquals(_weapons, null))
				{
					_weapons = new WeaponCollection(this);
				}
				return _weapons;
			}
		}

		/// <summary>
		/// Gets the last <see cref="Vehicle"/> the <see cref="Ped"/> used.
		/// </summary>
		/// <remarks>returns <langword>null</langword> if the Last Vehicle doesn't exist.</remarks>
		public Vehicle LastVehicle
		{
			get
			{
				int handle = Function.Call<int>(Hash.GET_VEHICLE_PED_IS_IN, Handle, true);

				if (!Function.Call<bool>(Hash.DOES_ENTITY_EXIST, handle))
				{
					return null;
				}

				return new Vehicle(handle);
			}
		}
		/// <summary>
		/// Gets the current <see cref="Vehicle"/> the <see cref="Ped"/> is using.
		/// </summary>
		/// <remarks>returns <langword>null</langword> if the <see cref="Ped"/> isn't in a <see cref="Vehicle"/>.</remarks>
		public Vehicle CurrentVehicle
		{
			get
			{
				if (!IsInVehicle())
				{
					return null;
				}

				return new Vehicle(Function.Call<int>(Hash.GET_VEHICLE_PED_IS_IN, Handle, false));
			}
		}
		/// <summary>
		/// Gets the <see cref="Vehicle"/> the <see cref="Ped"/> is trying to enter.
		/// </summary>
		/// <remarks>returns <langword>null</langword> if the <see cref="Ped"/> isn't in a <see cref="Vehicle"/>.</remarks>
		public Vehicle VehicleTryingToEnter
		{
			get
			{
				return Function.Call<Vehicle>(Hash.GET_VEHICLE_PED_IS_TRYING_TO_ENTER, Handle);
			}
		}
		/// <summary>
		/// Gets the PedGroup the <see cref="Ped"/> is in.
		/// </summary>
		public PedGroup PedGroup
		{
			get
			{
				if (!IsInGroup)
				{
					return null;
				}

				return new PedGroup(Function.Call<int>(Hash.GET_PED_GROUP_INDEX, Handle, false));
			}
		}

		/// <summary>
		/// Gets or sets the how much sweat should be rendered on the <see cref="Ped"/>.
		/// </summary>
		/// <value>
		/// The sweat from 0 to 100, 0 being no sweat, 100 being saturated.
		/// </value>
		public float Sweat
		{
			get
			{
				if (MemoryAddress == IntPtr.Zero)
				{
					return 0;
				}
				return MemoryAccess.ReadInt(MemoryAddress + 4464);
			}
			set
			{
				if (value < 0)
				{
					value = 0;
				}
				if (value > 100)
				{
					value = 100;
				}

				Function.Call(Hash.SET_PED_SWEAT, Handle, value);
			}
		}
		/// <summary>
		/// Sets how high up on the <see cref="Ped"/>s body water should be visible
		/// </summary>
		/// <value>
		/// The height ranges from 0.0f to 1.99f, 0.0f being no water visible, 1.99f being covered in water
		/// </value>
		public float WetnessHeight
		{
			set
			{
				if (value == 0.0f)
				{
					Function.Call(Hash.CLEAR_PED_WETNESS, Handle);
				}
				else
				{
					Function.Call<float>(Hash.SET_PED_WETNESS_HEIGHT, Handle, value);
				}
			}
		}

		/// <summary>
		/// Sets the voice to use when the <see cref="Ped"/> speaks.
		/// </summary>
		public string Voice
		{
			set
			{
				Function.Call(Hash.SET_AMBIENT_VOICE_NAME, Handle, value);
			}
		}

		/// <summary>
		/// Sets the rate the <see cref="Ped"/> will shoot at.
		/// </summary>
		/// <value>
		/// The shoot rate from 0.0f to 1000.0f, 100.0f is the default value
		/// </value>
		public int ShootRate
		{
			set
			{
				Function.Call(Hash.SET_PED_SHOOT_RATE, Handle, value);
			}
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="Ped"/> was killed by a stealth attack.
		/// </summary>
		/// <value>
		///   <c>true</c> if <see cref="Ped"/> was killed by stealth; otherwise, <c>false</c>.
		/// </value>
		public bool WasKilledByStealth
		{
			get
			{
				return Function.Call<bool>(Hash.WAS_PED_KILLED_BY_STEALTH, Handle);
			}
		}
		/// <summary>
		/// Gets a value indicating whether the <see cref="Ped"/> was killed by a takedown.
		/// </summary>
		/// <value>
		/// <c>true</c> if <see cref="Ped"/> was killed by a takedown; otherwise, <c>false</c>.
		/// </value>
		public bool WasKilledByTakedown
		{
			get
			{
				return Function.Call<bool>(Hash.WAS_PED_KILLED_BY_TAKEDOWN, Handle);
			}
		}

		/// <summary>
		/// Gets the <see cref="VehicleSeat"/> the <see cref="Ped"/> is in
		/// </summary>
		/// <value>
		/// The <see cref="VehicleSeat"/> the <see cref="Ped"/> is in if the <see cref="Ped"/> is in a <see cref="Vehicle"/>; otherwise, <see cref="VehicleSeat.None"/>
		/// </value>
		public VehicleSeat SeatIndex
		{
			get
			{
				if (MemoryAddress == IntPtr.Zero)
				{
					return VehicleSeat.None;
				}

				int seatIndex = MemoryAccess.ReadInt(MemoryAddress + 0x1542);

				if (seatIndex == -1 || !IsInVehicle())
				{
					return VehicleSeat.None;
				}

				return (VehicleSeat)(seatIndex - 1);
			}
		}
		/// <summary>
		/// Gets a value indicating whether this <see cref="Ped"/> is jumping out of their vehicle.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="Ped"/> is jumping out of their vehicle; otherwise, <c>false</c>.
		/// </value>
		public bool IsJumpingOutOfVehicle
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_JUMPING_OUT_OF_VEHICLE, Handle);
			}
		}
		/// <summary>
		/// Sets a value indicating whether this <see cref="Ped"/> will stay in the vehicle when the driver gets jacked
		/// </summary>
		/// <value>
		/// <c>true</c> if <see cref="Ped"/> stays in vehicle when jacked; otherwise, <c>false</c>.
		/// </value>
		public bool StaysInVehicleWhenJacked
		{
			set
			{
				Function.Call(Hash.SET_PED_STAY_IN_VEHICLE_WHEN_JACKED, Handle, value);
			}
		}

		/// <summary>
		/// Sets the maximum driving speed the <see cref="Ped"/> can drive at.
		/// </summary>
		public float MaxDrivingSpeed
		{
			set
			{
				Function.Call(Hash.SET_DRIVE_TASK_MAX_CRUISE_SPEED, Handle, value);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Ped"/> is human.
		/// </summary>
		/// <value>
		///   <c>true</c> if this <see cref="Ped"/> is human; otherwise, <c>false</c>.
		/// </value>
		public bool IsHuman
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_HUMAN, Handle);
			}
		}
		public bool IsEnemy
		{
			set
			{
				Function.Call(Hash.SET_PED_AS_ENEMY, Handle, value);
			}
		}
		public bool IsPriorityTargetForEnemies
		{
			set
			{
				Function.Call(Hash.SET_ENTITY_IS_TARGET_PRIORITY, Handle, value, 0);
			}
		}
		public bool IsPlayer
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_A_PLAYER, Handle);
			}
		}

		public bool IsCuffed
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_CUFFED, Handle);
			}
		}
		public bool IsWearingHelmet
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_WEARING_HELMET, Handle);
			}
		}

		public bool IsRagdoll
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_RAGDOLL, Handle);
			}
		}

		public bool IsIdle
		{
			get
			{
				return !IsInjured && !IsRagdoll && !IsInAir && !IsOnFire && !IsDucking && !IsGettingIntoAVehicle && !IsInCombat && !IsInMeleeCombat && (!IsInVehicle() || IsSittingInVehicle());
			}
		}
		public bool IsProne
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_PRONE, Handle);
			}
		}
		public bool IsDucking
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_DUCKING, Handle);
			}
			set
			{
				Function.Call(Hash.SET_PED_DUCKING, Handle, value);
			}
		}
		public bool IsGettingUp
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_GETTING_UP, Handle);
			}
		}
		public bool IsClimbing
		{

			get
			{
				return Function.Call<bool>(Hash.IS_PED_CLIMBING, Handle);
			}
		}
		public bool IsJumping
		{

			get
			{
				return Function.Call<bool>(Hash.IS_PED_JUMPING, Handle);
			}
		}
		public bool IsFalling
		{

			get
			{
				return Function.Call<bool>(Hash.IS_PED_FALLING, Handle);
			}
		}
		public bool IsStopped
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_STOPPED, Handle);
			}
		}
		public bool IsWalking
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_WALKING, Handle);
			}
		}
		public bool IsRunning
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_RUNNING, Handle);
			}
		}
		public bool IsSprinting
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_SPRINTING, Handle);
			}
		}
		public bool IsDiving
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_DIVING, Handle);
			}
		}
		public bool IsInParachuteFreeFall
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_IN_PARACHUTE_FREE_FALL, Handle);
			}
		}
		public bool IsSwimming
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_SWIMMING, Handle);
			}
		}
		public bool IsSwimmingUnderWater
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_SWIMMING_UNDER_WATER, Handle);
			}
		}
		public bool IsVaulting
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_VAULTING, Handle);
			}
		}

		public bool IsOnBike
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_ON_ANY_BIKE, Handle);
			}
		}
		public bool IsOnFoot
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_ON_FOOT, Handle);
			}
		}
		public bool IsInSub
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_IN_ANY_SUB, Handle);
			}
		}
		public bool IsInTaxi
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_IN_ANY_TAXI, Handle);
			}
		}
		public bool IsInTrain
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_IN_ANY_TRAIN, Handle);
			}
		}
		public bool IsInHeli
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_IN_ANY_HELI, Handle);
			}
		}
		public bool IsInPlane
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_IN_ANY_PLANE, Handle);
			}
		}
		public bool IsInFlyingVehicle
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_IN_FLYING_VEHICLE, Handle);
			}
		}
		public bool IsInBoat
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_IN_ANY_BOAT, Handle);
			}
		}
		public bool IsInPoliceVehicle
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_IN_ANY_POLICE_VEHICLE, Handle);
			}
		}

		public bool IsJacking
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_JACKING, Handle);
			}
		}
		public bool IsBeingJacked
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_BEING_JACKED, Handle);
			}
		}
		public bool IsGettingIntoAVehicle
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_GETTING_INTO_A_VEHICLE, Handle);
			}
		}
		public bool IsTryingToEnterALockedVehicle
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_TRYING_TO_ENTER_A_LOCKED_VEHICLE, Handle);
			}
		}

		public bool IsInjured
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_INJURED, Handle);
			}
		}
		public bool IsFleeing
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_FLEEING, Handle);
			}
		}

		public bool IsInCombat
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_IN_COMBAT, Handle);
			}
		}
		public bool IsInMeleeCombat
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_IN_MELEE_COMBAT, Handle);
			}
		}
		public bool IsShooting
		{

			get
			{
				return Function.Call<bool>(Hash.IS_PED_SHOOTING, Handle);
			}
		}
		public bool IsReloading
		{

			get
			{
				return Function.Call<bool>(Hash.IS_PED_RELOADING, Handle);
			}
		}
		public bool IsDoingDriveBy
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_DOING_DRIVEBY, Handle);
			}
		}
		public bool IsGoingIntoCover
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_GOING_INTO_COVER, Handle);
			}
		}
		public bool IsBeingStunned
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_BEING_STUNNED, Handle);
			}
		}
		public bool IsBeingStealthKilled
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_BEING_STEALTH_KILLED, Handle);
			}
		}
		public bool IsPerformingStealthKill
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_PERFORMING_STEALTH_KILL, Handle);
			}
		}

		public bool IsAimingFromCover
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_AIMING_FROM_COVER, Handle);
			}
		}
		public bool IsInCover()
		{
			return IsInCover(false);
		}
		public bool IsInCover(bool expectUseWeapon)
		{
			return Function.Call<bool>(Hash.IS_PED_IN_COVER, Handle, expectUseWeapon);
		}
		public bool IsInCoverFacingLeft
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_IN_COVER_FACING_LEFT, Handle);
			}
		}

		public string MovementAnimationSet
		{
			set
			{
				Function.Call(Hash.REQUEST_ANIM_SET, value);

				var endtime = DateTime.UtcNow + new TimeSpan(0, 0, 0, 0, 1000);

				while (!Function.Call<bool>(Hash.HAS_ANIM_DICT_LOADED, value))
				{
					Script.Yield();

					if (DateTime.UtcNow >= endtime)
					{
						return;
					}
				}

				Function.Call(Hash.SET_PED_MOVEMENT_CLIPSET, value, 1.0f);
			}
		}

		public FiringPattern FiringPattern
		{
			set
			{
				Function.Call(Hash.SET_PED_FIRING_PATTERN, Handle, value);
			}
		}
		public ParachuteLandingType ParachuteLandingType
		{
			get
			{
				return Function.Call<ParachuteLandingType>(Hash.GET_PED_PARACHUTE_LANDING_TYPE, Handle);
			}
		}
		public ParachuteState ParachuteState
		{
			get
			{
				return Function.Call<ParachuteState>(Hash.GET_PED_PARACHUTE_STATE, Handle);
			}
		}

		public bool DropsWeaponsOnDeath
		{
			get
			{
				if (MemoryAddress == IntPtr.Zero)
				{
					return false;
				}
				
				return (MemoryAccess.ReadByte(MemoryAddress + 0x13BD) & (1 << 6)) == 0;
			}
			set
			{
				Function.Call(Hash.SET_PED_DROPS_WEAPONS_WHEN_DEAD, Handle, value);
			}
		}

		public float DrivingSpeed
		{
			set
			{
				Function.Call(Hash.SET_DRIVE_TASK_CRUISE_SPEED, Handle, value);
			}
		}
		public DrivingStyle DrivingStyle
		{
			set
			{
				Function.Call(Hash.SET_DRIVE_TASK_DRIVING_STYLE, Handle, value);
			}
		}

		public bool CanRagdoll
		{
			get
			{
				return Function.Call<bool>(Hash.CAN_PED_RAGDOLL, Handle);
			}
			set
			{
				Function.Call(Hash.SET_PED_CAN_RAGDOLL, Handle, value);
			}
		}
		public bool CanPlayGestures
		{
			set
			{
				Function.Call(Hash.SET_PED_CAN_PLAY_GESTURE_ANIMS, Handle, value);
			}
		}
		public bool CanSwitchWeapons
		{
			set
			{
				Function.Call(Hash.SET_PED_CAN_SWITCH_WEAPON, Handle, value);
			}
		}
		public bool CanWearHelmet
		{
			set
			{
				Function.Call(Hash.SET_PED_HELMET, Handle, value);
			}
		}
		public bool CanBeTargetted
		{
			set
			{
				Function.Call(Hash.SET_PED_CAN_BE_TARGETTED, Handle, value);
			}
		}
		public bool CanBeShotInVehicle
		{
			set
			{
				Function.Call(Hash.SET_PED_CAN_BE_SHOT_IN_VEHICLE, Handle, value);
			}
		}
		public bool CanBeDraggedOutOfVehicle
		{
			set
			{
				Function.Call(Hash.SET_PED_CAN_BE_DRAGGED_OUT, Handle, value);
			}
		}
		public bool CanBeKnockedOffBike
		{
			set
			{
				Function.Call(Hash.SET_PED_CAN_BE_KNOCKED_OFF_VEHICLE, Handle, value);
			}
		}
		public bool CanFlyThroughWindscreen
		{
			get
			{
				return Function.Call<bool>(Hash.GET_PED_CONFIG_FLAG, Handle, 32, true);
			}
			set
			{
				Function.Call(Hash.SET_PED_CONFIG_FLAG, Handle, 32, value);
			}
		}
		public bool CanSufferCriticalHits
		{
			get
			{
				if (MemoryAddress == IntPtr.Zero)
				{
					return false;
				}
				
				return (MemoryAccess.ReadByte(MemoryAddress + 0x13BC) & (1 << 2)) == 0;
			}
			set
			{
				Function.Call(Hash.SET_PED_SUFFERS_CRITICAL_HITS, Handle, value);
			}
		}
		public bool CanWrithe
		{
			get
			{
				return !GetConfigFlag(281);
			}
			set
			{
				SetConfigFlag(281, !value);
			}
		}
		/// <summary>
		/// Sets whether permanent events are blocked for this <see cref="Ped"/>.
		///  If permanent events are blocked, this <see cref="Ped"/> will only do as it's told, and won't flee when shot at, etc.
		/// </summary>
		/// <value>
		///   <c>true</c> if permanent events are blocked; otherwise, <c>false</c>.
		/// </value>
		public bool BlockPermanentEvents
		{
			set
			{
				Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, Handle, value);
			}
		}

		public bool AlwaysKeepTask
		{
			set
			{
				Function.Call(Hash.SET_PED_KEEP_TASK, Handle, value);
			}
		}
		public bool AlwaysDiesOnLowHealth
		{
			set
			{
				Function.Call(Hash.SET_PED_DIES_WHEN_INJURED, Handle, value);
			}
		}
		public bool DrownsInWater
		{
			set
			{
				Function.Call(Hash.SET_PED_DIES_IN_WATER, Handle, value);
			}
		}
		public bool DrownsInSinkingVehicle
		{
			set
			{
				Function.Call(Hash.SET_PED_DIES_IN_SINKING_VEHICLE, Handle, value);
			}
		}
		public bool DiesInstantlyInWater
		{
			set
			{
				Function.Call(Hash.SET_PED_DIES_INSTANTLY_IN_WATER, Handle, value);
			}
		}

		public bool IsInVehicle()
		{
			return Function.Call<bool>(Hash.IS_PED_IN_ANY_VEHICLE, Handle, 0);
		}
		public bool IsInVehicle(Vehicle vehicle)
		{
			return Function.Call<bool>(Hash.IS_PED_IN_VEHICLE, Handle, vehicle.Handle, 0);
		}
		public bool IsSittingInVehicle()
		{
			return Function.Call<bool>(Hash.IS_PED_SITTING_IN_ANY_VEHICLE, Handle);
		}
		public bool IsSittingInVehicle(Vehicle vehicle)
		{
			return Function.Call<bool>(Hash.IS_PED_SITTING_IN_VEHICLE, Handle, vehicle.Handle);
		}
		public void SetIntoVehicle(Vehicle vehicle, VehicleSeat seat)
		{
			Function.Call(Hash.SET_PED_INTO_VEHICLE, Handle, vehicle.Handle, seat);
		}

		public Relationship GetRelationshipWithPed(Ped ped)
		{
			return (Relationship)Function.Call<int>(Hash.GET_RELATIONSHIP_BETWEEN_PEDS, Handle, ped.Handle);
		}

		public bool IsHeadtracking(Entity entity)
		{
			return Function.Call<bool>(Hash.IS_PED_HEADTRACKING_ENTITY, Handle, entity.Handle);
		}
		public bool IsInCombatAgainst(Ped target)
		{
			return Function.Call<bool>(Hash.IS_PED_IN_COMBAT, Handle, target.Handle);
		}

		public Ped GetJacker()
		{
			return new Ped(Function.Call<int>(Hash.GET_PEDS_JACKER, Handle));
		}
		public Ped GetJackTarget()
		{
			return new Ped(Function.Call<int>(Hash.GET_JACK_TARGET, Handle));
		}
		public Ped GetMeleeTarget()
		{
			return new Ped(Function.Call<int>(Hash.GET_MELEE_TARGET_FOR_PED, Handle));
		}
		public Entity GetKiller()
		{
			int entity = Function.Call<int>(Hash._GET_PED_KILLER, Handle);

			if (Function.Call<bool>(Hash.DOES_ENTITY_EXIST, entity))
			{
				switch (Function.Call<int>(Hash.GET_ENTITY_TYPE, entity))
				{
					case 1:
						return new Ped(entity);
					case 2:
						return new Vehicle(entity);
					case 3:
						return new Prop(entity);
				}
			}

			return null;
		}

		public void Kill()
		{
			Health = -1;
		}

		public void ResetVisibleDamage()
		{
			Function.Call(Hash.RESET_PED_VISIBLE_DAMAGE, Handle);
		}
		public void ClearBloodDamage()
		{
			Function.Call(Hash.CLEAR_PED_BLOOD_DAMAGE, Handle);
		}

		public void RandomizeOutfit()
		{
			Function.Call(Hash.SET_PED_RANDOM_COMPONENT_VARIATION, Handle, false);
		}
		public void SetDefaultClothes()
		{
			Function.Call(Hash.SET_PED_DEFAULT_COMPONENT_VARIATION, Handle);
		}

		public RelationshipGroup RelationshipGroup
		{
			get
			{
				return new GTA.RelationshipGroup(Function.Call<int>(Hash.GET_PED_RELATIONSHIP_GROUP_HASH, Handle));
			}
			set
			{
				Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, Handle, value.Hash);
			}
		}
		public bool IsInGroup
		{
			get
			{
				return Function.Call<bool>(Hash.IS_PED_IN_GROUP, Handle);
			}
		}
		public bool NeverLeavesGroup
		{
			set
			{
				Function.Call(Hash.SET_PED_NEVER_LEAVES_GROUP, Handle, value);
			}
		}
		public void LeaveGroup()
		{
			Function.Call(Hash.REMOVE_PED_FROM_GROUP, Handle);
		}

		public void ApplyDamage(int damageAmount)
		{
			Function.Call(Hash.APPLY_DAMAGE_TO_PED, Handle, damageAmount, true);
		}

		public int GetBoneIndex(Bone BoneID)
		{
			return Function.Call<int>(Hash.GET_PED_BONE_INDEX, Handle, BoneID);
		}
		public Vector3 GetBoneCoord(Bone BoneID)
		{
			return GetBoneCoord(BoneID, Vector3.Zero);
		}
		public Vector3 GetBoneCoord(Bone BoneID, Vector3 Offset)
		{
			return Function.Call<Vector3>(Hash.GET_PED_BONE_COORDS, Handle, BoneID, Offset.X, Offset.Y, Offset.Z);
		}

		public Vector3 GetLastWeaponImpactPosition()
		{
			var positionArg = new OutputArgument();

			if (Function.Call<bool>(Hash.GET_PED_LAST_WEAPON_IMPACT_COORD, Handle, positionArg))
			{
				return positionArg.GetResult<Vector3>();
			}

			return Vector3.Zero;
		}

		public void GiveHelmet(bool canBeRemovedByPed, HelmetType helmetType, int textureIndex)
		{
			Function.Call(Hash.GIVE_PED_HELMET, Handle, !canBeRemovedByPed, helmetType, textureIndex);
		}
		public void RemoveHelmet(bool instantly)
		{
			Function.Call(Hash.REMOVE_PED_HELMET, Handle, instantly);
		}

		public void OpenParachute()
		{
			Function.Call(Hash.FORCE_PED_TO_OPEN_PARACHUTE, Handle);
		}

		public bool GetConfigFlag(int flagID)
		{
			return Function.Call<bool>(Hash.GET_PED_CONFIG_FLAG, Handle, flagID, true);
		}
		public void SetConfigFlag(int flagID, bool value)
		{
			Function.Call(Hash.SET_PED_CONFIG_FLAG, Handle, flagID, value);
		}
		public void ResetConfigFlag(int flagID)
		{
			Function.Call(Hash.SET_PED_RESET_FLAG, Handle, flagID, true);
		}

		public Ped Clone()
		{
			return Clone(0f);
		}
		public Ped Clone(float heading)
		{
			return new Ped(Function.Call<int>(Hash.CLONE_PED, Handle, heading, false, false));
		}
	}
}
