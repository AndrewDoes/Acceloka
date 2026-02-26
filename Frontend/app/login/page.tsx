"use client";

import { useState } from "react";
import { GoogleOutlined } from "@ant-design/icons";

export default function LoginPage() {
    const [isLoading, setIsLoading] = useState(false);

    const handleGoogleLogin = () => {
        setIsLoading(true);

        // TODO: Redirect to your actual ASP.NET Core Google Challenge endpoint
        window.location.href = 'http://localhost:5225/api/v1/auth/login-google';

        setIsLoading(false);
    };

    return (
        <div className="h-200 flex items-center justify-center bg-acceloka-bg px-4 sm:px-6 lg:px-8">
            <div className="max-w-md w-full bg-acceloka-surface p-8 rounded-2xl shadow-lg border border-acceloka-border">

                {/* Header */}
                <div className="text-center mb-8">
                    <h2 className="text-3xl font-extrabold text-acceloka-blue tracking-tight mb-2">Welcome Back</h2>
                    <p className="text-sm text-acceloka-muted font-medium">Sign in to book and manage your tickets</p>
                </div>

                {/* OpenID / Google Login Button */}
                <div className="space-y-6">
                    <button
                        onClick={handleGoogleLogin}
                        disabled={isLoading}
                        className="w-full bg-white border border-slate-300 text-slate-700 font-bold py-3.5 rounded-xl hover:bg-slate-50 active:scale-[0.98] transition-all shadow-sm flex justify-center items-center gap-3 disabled:opacity-70 disabled:cursor-not-allowed"
                    >
                        {isLoading ? (
                            <div className="w-5 h-5 border-2 border-acceloka-blue border-t-transparent rounded-full animate-spin"></div>
                        ) : (
                            <>
                                <GoogleOutlined className="!text-acceloka-blue text-xl" />
                                Continue with Google
                            </>
                        )}
                    </button>
                </div>

                {/* Security Note */}
                <p className="mt-8 text-center text-xs text-acceloka-muted font-medium leading-relaxed">
                    By signing in, you agree to Acceloka's <a href="#" className="text-acceloka-blue hover:underline">Terms of Service</a> and <a href="#" className="text-acceloka-blue hover:underline">Privacy Policy</a>.
                </p>
            </div>
        </div>
    );
}