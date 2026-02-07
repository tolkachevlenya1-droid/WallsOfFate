using System;

namespace Player {
    public class Stats {
        private int _freePoints;
        private int _strength;
        private int _int;
        private int _dex;
        private int _percept;
        private int _mystic;

        public int FreePoints {
            get => _freePoints;
            set {
                if (_freePoints != value) _freePoints = Math.Max(value, 0);
            }
        }

        public int Strength {
            get => _strength;
            set {
                if (_strength != value) _strength = Math.Max(value, 0);
            }
        }

        public int Int {
            get => _int;
            set {
                if (_int != value) _int = Math.Max(value, 0);
            }
        }

        public int Dex {
            get => _dex;
            set {
                if (_dex != value) _dex = Math.Max(value, 0);
            }
        }

        public int Percept {
            get => _percept;
            set {
                if (_percept != value) _percept = Math.Max(value, 0);
            }
        }

        public int Mystic {
            get => _mystic;
            set {
                if (_mystic != value) _mystic = Math.Max(value, 0);
            }
        }

        public void AddFreePoints(int delta) { FreePoints += delta; }

        public void AddStrength(int delta) { Strength += delta; }

        public void AddInt(int delta) { Int += delta; }
        public void AddDex(int delta) { Dex += delta; }

        public void AddPerceept(int delta) { Percept += delta; }

        public void AddMystic(int delta) { Mystic += delta; }

        public void SetInitialPoints(int points) { FreePoints += points; }
    }
}
