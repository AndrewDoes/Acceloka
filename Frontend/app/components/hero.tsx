"use client";

import React, { useState, useEffect } from 'react';

const HERO_IMAGES = [
    "https://images.unsplash.com/photo-1459749411175-04bf5292ceea?ixlib=rb-4.0.3&auto=format&fit=crop&w=1920&q=80", // Concert
    "https://images.unsplash.com/photo-1727873275022-d5189075c660?q=80&w=1548&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D", // Football/Sport
    "https://images.unsplash.com/photo-1533174072545-7a4b6ad7a6c3?ixlib=rb-4.0.3&auto=format&fit=crop&w=1920&q=80", // Festival
];

export default function HeroSection() {
    const [currentIndex, setCurrentIndex] = useState(0);

    // Auto-slide logic
    useEffect(() => {
        const timer = setInterval(() => {
            setCurrentIndex((prevIndex) => (prevIndex + 1) % HERO_IMAGES.length);
        }, 5000); // Change image every 5 seconds

        return () => clearInterval(timer);
    }, []);

    return (
        <section className="relative bg-acceloka-bg text-white pt-24 pb-40 px-4 overflow-hidden transition-colors duration-300">

            {/* Background Image Stack for Cross-fade */}
            {HERO_IMAGES.map((img, index) => (
                <div
                    key={img}
                    className={`absolute inset-0 bg-cover bg-center transition-opacity duration-1000 ease-in-out ${index === currentIndex ? "opacity-100" : "opacity-0"
                        }`}
                    style={{ backgroundImage: `url('${img}')` }}
                />
            ))}

            {/* Themed Overlay (Matches your globals.css variable) */}
            <div className="absolute inset-0 bg-(--acceloka-hero-overlay) mix-blend-multiply transition-colors duration-300"></div>

            {/* Smooth gradient fade into the page background */}
            <div className="absolute inset-0 bg-linear-to-t from-acceloka-bg via-transparent to-transparent opacity-100"></div>

            {/* Content */}
            <div className="relative max-w-4xl mx-auto text-center z-10">
                <h1 className="text-5xl md:text-6xl font-extrabold mb-6 text-white drop-shadow-2xl tracking-tight leading-tight">
                    Book Your Ticket <span className="text-acceloka-blue">Anywhere</span>
                </h1>
                <p className="text-(--acceloka-hero-subtext) text-lg md:text-xl drop-shadow-lg font-medium max-w-2xl mx-auto leading-relaxed opacity-90 mb-8">
                    The ultimate platform for concerts, sports, and festivals. Experience enterprise-grade security with every booking.
                </p>

                {/* Carousel Indicators */}
                <div className="flex justify-center gap-3 mt-4">
                    {HERO_IMAGES.map((_, index) => (
                        <button
                            key={index}
                            onClick={() => setCurrentIndex(index)}
                            className={`h-1.5 rounded-full transition-all duration-300 ${index === currentIndex
                                ? "w-8 bg-acceloka-blue"
                                : "w-2 bg-white/30 hover:bg-white/50"
                                }`}
                            aria-label={`Go to slide ${index + 1}`}
                        />
                    ))}
                </div>
            </div>
        </section>
    );
}