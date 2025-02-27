import { useState, useEffect } from 'react';
import { getUser } from './userService.js'
import AuthContext from './AuthContext'
export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    useEffect(() => {
        getUser()
            .then(response => { setUser(response) })
            .catch(e => setUser({ isAuthenticated: false }))
    }, []);

    return (
        <AuthContext.Provider value={{ user }}>{children}</AuthContext.Provider>
    );
};