'use client';

import { createContext, useContext, useEffect, useState } from "react";
import { authService } from "../services/api";

interface AuthContextType {
    isLoggedIn: boolean;
    userName: string;
    userEmail: string;
    role: string; // Added role property
    isCheckingAuth: boolean;
    checkStatus: () => Promise<void>;
    logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [userName, setUserName] = useState("");
    const [userEmail, setUserEmail] = useState("");
    const [role, setRole] = useState(""); // Added role state
    const [isCheckingAuth, setIsCheckingAuth] = useState(true);

    const checkStatus = async () => {
        try {
            const data = await authService.checkAuthStatus();
            if (data?.isAuthenticated) {
                setIsLoggedIn(true);
                setUserName(data.name || data.email?.split('@')[0] || "User");
                setUserEmail(data.email || "");
                setRole(data.role || "User"); // Extracting role from backend
            } else {
                setIsLoggedIn(false);
                setRole("");
            }
        } catch (error) {
            setIsLoggedIn(false);
            setRole("");
        } finally {
            setIsCheckingAuth(false);
        }
    }

    const logout = async () => {
        try {
            await authService.logout();
        } finally {
            setIsLoggedIn(false);
            setUserName("");
            setUserEmail("");
            setRole("");
        }
    }

    useEffect(() => {
        checkStatus();
    }, []);

    return (
        <AuthContext.Provider value={{
            isLoggedIn,
            userName,
            userEmail,
            role,
            isCheckingAuth,
            checkStatus,
            logout
        }}>
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