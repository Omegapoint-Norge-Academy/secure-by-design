import { useAuthContext } from "../auth/useAuthContext";
import React from "react";
import { useLocation } from "react-router-dom";

export function Authentication() {
    const context = useAuthContext();
    const location = useLocation();

    return context?.user?.isAuthenticated
        ? <a href="client/account/logout">
            click here to logout (logged in as {context?.user?.claims?.find(x => x.key === 'name')?.value})</a>
        : <a href={`client/account/login?returnUrl=${location.pathname}`}>click here to login</a>;
}