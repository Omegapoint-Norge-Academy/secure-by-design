import AuthContext from "./AuthContext";
import { useContext } from "react";
export const useAuthContext = () => {
    const user = useContext(AuthContext);
    if (user === undefined) {
        throw new Error("useAuthContext can only be used inside AuthProvider");
    }
    return user;
};