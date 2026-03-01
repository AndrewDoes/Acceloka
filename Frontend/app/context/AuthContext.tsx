'use client';

import { createContext, useContext, useEffect, useState } from "react";
import { authService } from "../services/api";

interface AuthContextType {
    isLoggedIn: boolean;
    userName: string;
    userEmail: string;
    isCheckingAuth: boolean;
    checkStatus: () => Promise<void>;
    logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [userName, setUserName] = useState("");
    const [userEmail, setUserEmail] = useState("");
    const [isCheckingAuth, setIsCheckingAuth] = useState(true);

    const checkStatus = async () => {
        try {
            const data = await authService.checkAuthStatus();
            if (data?.isAuthenticated) {
                setIsLoggedIn(true);
                setUserName(data.name || data.email?.split('@')[0] || "User");
                setUserEmail(data.email || "");
            } else {
                setIsLoggedIn(false);
            }
        } catch (error) {
            setIsLoggedIn(false);
        } finally {
            setIsCheckingAuth(false);
        }
    }

    const logout = async () => {
        await authService.logout();
        setIsLoggedIn(false);
        setUserName("");
    }

    useEffect(() => {
        checkStatus();
    }, []);

    return (
        <AuthContext.Provider value={{ isLoggedIn, userName, userEmail, isCheckingAuth, checkStatus, logout }}>
            {children}
        </AuthContext.Provider>
    )
}

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error("useAuth must be used within an AuthProvider");
    }
    return context;
}