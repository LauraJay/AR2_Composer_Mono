public class Marker {

        private int ID;
        private float posX;
        private float posY;
        private float angle;
        private int status;

        public Marker(int ID, float posX, float posY, float angle, int status){
            this.ID = ID;
            this.posX = posX;
            this.posY = posY;
            this.angle = angle;
            this.status = status;
        }

        public int getID(){
            return this.ID;
        }

        public float getPosX(){
            return this.posX;
        }

        public float getPosY(){
            return this.posY;
        }

        public float getAngle(){
            return this.angle;
        }

        public int getStatus(){
            return this.status;
        }

        public void setID(int ID){
            this.ID = ID;
        }

        public void setPosX(float posX){
            this.posX = posX;
        }

        public void setPosY(float posY){
            this.posY = posY;
        }

        public void setAngle(float angle){
            this.angle = angle;
        }

        public void setStatus(int status){
            this.status = status;
        }

        public string toStr(){
            return "Marker " + this.ID + " data:\n" +
                "\tPosition: (" + this.posX + "/" + this.posY + ")\n" +
                "\tAngle: " + this.angle + "\n\tStatus: " + status;
        }

    void Start () {}

    void Update () {}
}
