"use client";

import { useState, useEffect } from "react";
import { MenuOutlined, CloseOutlined, UserOutlined, LogoutOutlined } from "@ant-design/icons";
import { useAuth } from "../context/AuthContext";
import { usePathname } from "next/navigation";

export default function Navbar() {
    const pathname = usePathname();
    const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

    // Auth State
    const [isProfileDropdownOpen, setIsProfileDropdownOpen] = useState(false);
    const { isLoggedIn, userName, logout, isCheckingAuth } = useAuth();

    const navLinks = [
        { name: "Explore Tickets", href: "/" },
        { name: "My Bookings", href: "/my-bookings" },
    ];

    return (
        <div className="nav gap-0 relative z-50">
            <div className="navbar flex flex-col bg-acceloka-surface shadow-sm items-center lg:flex-row lg:justify-between lg:px-8 xl:px-12 lg:py-4 py-4 border-b border-acceloka-border">

                {/* phone & tablets */}
                <div className="flex w-full justify-between items-center px-6 lg:hidden">
                    <a href="/" className="logo text-2xl text-acceloka-blue font-extrabold tracking-tight">Acceloka</a>
                    <button
                        onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
                        className="text-xl text-acceloka-text focus:outline-none"
                    >
                        {isMobileMenuOpen ? <CloseOutlined /> : <MenuOutlined />}
                    </button>
                </div>

                {/* desktop & tablets-landscape */}
                <div className="hidden justify-between items-center w-screen lg:flex px-20 mx-auto">
                    <a href="/" className="logo text-2xl xl:text-3xl text-acceloka-blue font-extrabold tracking-tight">Acceloka</a>

                    <div className="flex items-center gap-12 xl:gap-24">
                        <ul className="nav-list-lg flex gap-6 lg:gap-12 xl:gap-24 text-sm xl:text-base font-semibold tracking-wide text-center items-center">
                            {navLinks.map((link) => (
                                <li key={link.href} className="links">
                                    <a
                                        href={link.href}
                                        className={`transition-colors duration-200 pb-1 ${pathname === link.href
                                            ? "text-acceloka-text border-b-[3px] border-acceloka-text"
                                            : "text-acceloka-muted hover:text-acceloka-text"
                                            }`}
                                    >
                                        {link.name}
                                    </a>
                                </li>
                            ))}
                        </ul>

                        {!isCheckingAuth && (
                            isLoggedIn ? (
                                <div className="relative">
                                    <button className="flex items-center gap-2.5 bg-acceloka-blue text-white pl-1.5 pr-4 py-1.5 rounded-full max-w-50 overflow-x-hidden text-sm font-semibold hover:bg-blue-600 transition shadow-sm"
                                        onClick={() => setIsProfileDropdownOpen(!isProfileDropdownOpen)}>
                                        <div className="bg-white rounded-full w-7 h-7 flex items-center justify-center">
                                            <UserOutlined className="text-acceloka-blue text-sm" />
                                        </div>
                                        <span className="truncate max-w-32">{userName}</span>
                                    </button>

                                    {isProfileDropdownOpen && (
                                        <div className="absolute right-0 mt-4 w-48 bg-acceloka-surface border border-acceloka-border rounded-xl shadow-lg py-2 animate-in fade-in slide-in-from-top
                                                        hover:border-acceloka-accent hover:bg-acceloka-bg/80 transition-colors
                                        ">
                                            <button
                                                onClick={logout}
                                                className="w-full text-left px-5 py-2.5 text-sm text-acceloka-accent hover:bg-acceloka-bg/80 font-bold flex items-center gap-3 transition-colors">
                                                <LogoutOutlined />
                                                Log out
                                            </button>
                                        </div>
                                    )}
                                </div>
                            ) : (
                                <div className="flex items-center gap-4 border-l border-acceloka-border pl-8">
                                    <a href="/login" className="text-acceloka-text font-bold text-sm hover:text-acceloka-blue transition-colors">
                                        Log in
                                    </a>
                                    <a href="/login" className="bg-acceloka-blue text-white px-5 py-2 rounded-full text-sm font-bold hover:bg-blue-600 transition-colors shadow-sm">
                                        Sign up
                                    </a>
                                </div>
                            )
                        )}
                    </div>
                </div>
            </div>

            {/* Mobile Menu Dropdown */}
            <div className={`lg:hidden bg-acceloka-surface w-full overflow-hidden transition-all duration-300 ease-in-out ${isMobileMenuOpen ? "max-h-96 border-b border-acceloka-border shadow-md" : "max-h-0"
                }`}>
                <ul className="flex flex-col text-base font-semibold">
                    {navLinks.map((link) => (
                        <li key={link.href} className="w-full">
                            <a
                                href={link.href}
                                onClick={() => setIsMobileMenuOpen(false)}
                                className={`block w-full px-6 py-4 transition-colors ${pathname === link.href
                                    ? "text-acceloka-blue bg-blue-50"
                                    : "text-acceloka-muted hover:bg-slate-50"
                                    }`}
                            >
                                {link.name}
                            </a>
                        </li>
                    ))}

                    {!isCheckingAuth && (
                        isLoggedIn ? (
                            <div className="relative">
                                <li className="w-full border-t border-acceloka-border mt-1 pt-1">
                                    <a href="/profile" className="flex items-center gap-3 w-full px-6 py-4 text-acceloka-text hover:bg-slate-50 transition-colors">
                                        <div className="bg-acceloka-blue rounded-full w-8 h-8 flex items-center justify-center text-white">
                                            <UserOutlined />
                                        </div>
                                        {userName}
                                    </a>
                                </li>
                                <li className="w-full border-t border-acceloka-border mt-1 py-4 px-6">
                                    <button
                                        onClick={logout}
                                        className="flex items-center text-acceloka-accent bg-acceloka-bg gap-4 hover:bg-acceloka-bg transition-colors text-left font-bold">
                                        <LogoutOutlined />
                                        Log Out
                                    </button>
                                </li>
                            </div>

                        ) : (
                            <li className="w-full border-t border-acceloka-border mt-1 p-4 flex flex-col gap-3">
                                <a href="/login" className="w-full text-center border border-acceloka-blue text-acceloka-blue font-bold py-2.5 rounded-lg hover:bg-blue-50 transition-colors">
                                    Log in / Sign up
                                </a>
                            </li>
                        )
                    )}
                </ul>
            </div>
        </div >
    );
}