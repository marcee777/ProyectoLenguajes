namespace ProyectoLenguajes.Utilities
{
    /**
     * Clase StaticValues
     * 
     * Contiene constantes estáticas que representan roles de usuario, nombres por defecto de imágenes
     * y los diferentes estados que puede tener una orden dentro del sistema. Estas constantes se utilizan
     * para mantener la coherencia y evitar valores "mágicos" dispersos en el código.
     * 
     * @author Melanie Arce C30634
     * @author Carolina Rodriguez C36640
     * @author Marcela Rojas C36975
     * @version 07/07/25
     */

    public static class StaticValues
    {

        public const string Role_Admin = "Admin";
        public const string Role_Kitchen = "Kitchen";
        public const string Role_Customer = "Customer";

        public const string Image_DefaultName = "default.jpg";

        public const string Status_Unconfirmed = "Unconfirmed";
        public const string Status_OnTime = "On Time";
        public const string Status_OverTime = "Over Time";
        public const string Status_Delayed = "Delayed";
        public const string Status_Canceled = "Canceled";
        public const string Status_Delivered = "Delivered";
    }
}
