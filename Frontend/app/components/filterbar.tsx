"use client";

import React from 'react';
import { Search, ChevronDown, Calendar, Banknote } from 'lucide-react';

export interface TicketFilters {
    categoryName: string;
    ticketCode: string;
    ticketName: string;
    maxPrice: string | number;
    startEventDate: string;
    endEventDate: string;
    orderBy: string;
    orderState: string;
}

interface FilterBarProps {
    filters: TicketFilters;
    updateFilter: (key: keyof TicketFilters, value: string) => void;
    onSearch: () => void;
}

const FilterBar: React.FC<FilterBarProps> = ({ filters, updateFilter, onSearch }) => {
    const handleKeyDown = (e: React.KeyboardEvent) => {
        if (e.key === 'Enter') onSearch();
    };

    return (
        <div className="max-w-7xl w-full mx-auto px-4 sm:px-6 lg:px-8 -mt-32 relative z-20 mb-10">
            <div className="bg-acceloka-surface rounded-2xl shadow-xl p-6 md:p-10 border border-acceloka-border transition-all duration-300">

                {/* Search & Category */}
                <div className="flex flex-col lg:flex-row gap-8 mb-10 items-end">
                    <div className="grow relative group w-full">
                        <div className="absolute left-7 inset-y-0 flex items-center pointer-events-none">
                            <Search
                                strokeWidth={2.5}
                                className="w-5 h-5 text-acceloka-muted group-focus-within:text-acceloka-blue transition-colors"
                            />
                        </div>
                        <input
                            type="text"
                            value={filters.ticketName}
                            onChange={(e) => updateFilter("ticketName", e.target.value)}
                            onKeyDown={handleKeyDown}
                            placeholder="Search for events, concerts, or artists (Press Enter)..."
                            className="w-full pl-16 pr-8 py-4 border border-acceloka-border text-acceloka-text rounded-full focus:ring-2 focus:ring-acceloka-blue/20 focus:border-acceloka-blue outline-none transition-all text-base bg-acceloka-bg placeholder-acceloka-muted/60 font-medium"
                        />
                    </div>

                    <div className="w-full lg:w-80">
                        <label className="block text-[10px] text-acceloka-muted uppercase font-bold mb-2 ml-6 tracking-widest">Category</label>
                        <div className="relative">
                            <select
                                value={filters.categoryName}
                                onChange={(e) => {
                                    updateFilter("categoryName", e.target.value);
                                }}
                                className="w-full pl-6 pr-12 py-4 border border-acceloka-border rounded-full text-acceloka-text bg-acceloka-bg focus:ring-2 focus:ring-acceloka-blue/20 focus:border-acceloka-blue outline-none text-sm font-bold appearance-none cursor-pointer transition-all"
                            >
                                <option value="">All Categories</option>
                                <option value="Sport">Sports</option>
                                <option value="Concert">Concert</option>
                                <option value="Festival">Festival</option>
                                <option value="Exhibition">Exhibition</option>
                            </select>
                            <div className="absolute right-6 inset-y-0 flex items-center pointer-events-none">
                                <ChevronDown className="w-4 h-4 text-acceloka-muted" />
                            </div>
                        </div>
                    </div>
                </div>

                {/* Specific Filters */}
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-12 gap-8 items-center border-t border-acceloka-border/40 pt-10">

                    {/* Start Date */}
                    <div className="lg:col-span-3 group/date">
                        <label className="block text-[10px] text-acceloka-muted uppercase font-bold mb-3 tracking-widest group-hover/date:text-acceloka-blue transition-colors">Start Date</label>
                        <input
                            type="date"
                            value={filters.startEventDate}
                            onChange={(e) => updateFilter("startEventDate", e.target.value)}
                            onClick={(e) => (e.target as HTMLInputElement).showPicker?.()} // Trigger the picker when clicking anywhere on the input (text area)
                            className="w-full pb-2 border-b-2 border-acceloka-border text-acceloka-text bg-transparent focus:border-acceloka-blue outline-none text-sm font-bold transition-colors hover:border-acceloka-muted cursor-pointer appearance-none"
                        />
                    </div>

                    <div className="hidden lg:block lg:col-span-1 justify-self-center opacity-30">
                        <div className="h-10 w-px bg-acceloka-border"></div>
                    </div>

                    {/* End Date */}
                    <div className="lg:col-span-3 group/date">
                        <label className="block text-[10px] text-acceloka-muted uppercase font-bold mb-3 tracking-widest group-hover/date:text-acceloka-blue transition-colors">End Date</label>
                        <input
                            type="date"
                            value={filters.endEventDate}
                            onChange={(e) => updateFilter("endEventDate", e.target.value)}
                            onClick={(e) => (e.target as HTMLInputElement).showPicker?.()}
                            className="w-full pb-2 border-b-2 border-acceloka-border text-acceloka-text bg-transparent focus:border-acceloka-blue outline-none text-sm font-bold transition-colors hover:border-acceloka-muted cursor-pointer appearance-none"
                        />
                    </div>

                    <div className="hidden lg:block lg:col-span-1 justify-self-center opacity-30">
                        <div className="h-10 w-px bg-acceloka-border"></div>
                    </div>

                    {/* Max Price */}
                    <div className="lg:col-span-2">
                        <label className="block text-[10px] text-acceloka-muted uppercase font-bold mb-3 tracking-widest">Max Price</label>
                        <div className="flex items-center text-sm text-acceloka-text border-b-2 border-acceloka-border pb-2 focus-within:border-acceloka-blue transition-colors group">
                            <span className="mr-2 font-bold text-acceloka-muted opacity-40">Rp.</span>
                            <input
                                type="number"
                                value={filters.maxPrice}
                                onChange={(e) => updateFilter("maxPrice", e.target.value)}
                                placeholder="Budget"
                                className="w-full bg-transparent outline-none text-sm font-bold placeholder-acceloka-muted/30"
                            />
                        </div>
                    </div>

                    {/* Search Button */}
                    <div className="lg:col-span-2">
                        <button
                            onClick={onSearch}
                            className="w-full bg-acceloka-blue text-white py-4 rounded-xl font-bold text-sm hover:brightness-110 active:scale-[0.96] transition-all shadow-lg shadow-acceloka-blue/25 flex items-center justify-center gap-2"
                        >
                            Apply Filters
                        </button>
                    </div>

                </div>
            </div>
        </div>
    );
};

export default FilterBar;