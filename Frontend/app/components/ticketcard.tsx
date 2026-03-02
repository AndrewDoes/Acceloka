"use client";

import React from 'react';
import { Card, Tag, Button, Typography, Space } from 'antd';
import {
    CalendarOutlined,
    PlusOutlined,
    TagOutlined,
    ExclamationCircleOutlined
} from '@ant-design/icons';

const { Title, Text } = Typography;

/**
 * TICKET DATA INTERFACE
 */
export interface TicketData {
    ticketCode?: string;
    ticketName?: string;
    categoryName?: string;
    category?: string;
    eventDate?: string;
    quota?: number;
    price?: number;
    TicketCode?: string;
    TicketName?: string;
    CategoryName?: string;
    Category?: string;
    EventDate?: string;
    Quota?: number;
    Price?: number;
    pageSize?: number;
    quantity?: number;
}

interface TicketCardProps {
    ticket: TicketData;
    onSelect: (ticket: TicketData) => void;
    onAdd: (ticket: TicketData) => void;
}

const TicketCard: React.FC<TicketCardProps> = ({ ticket, onSelect, onAdd }) => {
    const getAttr = (key: string) => {
        const pascalKey = key.charAt(0).toUpperCase() + key.slice(1);
        return (ticket as any)[key] ?? (ticket as any)[pascalKey];
    };

    const ticketCode = getAttr('ticketCode');
    const ticketName = getAttr('ticketName');
    const categoryName = getAttr('categoryName') || getAttr('category') || 'Event';
    const eventDate = getAttr('eventDate');
    const quota = getAttr('quota') ?? 0;
    const price = getAttr('price') ?? 0;

    const formatPrice = (amount: number) => {
        return new Intl.NumberFormat('id-ID', {
            style: 'currency',
            currency: 'IDR',
            minimumFractionDigits: 0,
        }).format(amount);
    };

    const isLowQuota = quota > 0 && quota < 50;
    const isSoldOut = quota === 0;

    const getCategoryColor = (cat: string) => {
        switch (cat?.toLowerCase()) {
            case 'concert': return 'blue';
            case 'sport': return 'green';
            case 'festival': return 'purple';
            case 'exhibition': return 'cyan';
            default: return 'orange';
        }
    };

    /**
     * CATEGORIZED IMAGE MAPPING
     */
    const getCategoryImage = (cat: string) => {
        const category = cat?.toLowerCase();
        if (category?.includes('concert'))
            return "https://images.unsplash.com/photo-1540039155733-5bb30b53aa14?auto=format&fit=crop&w=800&q=80";
        if (category?.includes('sport'))
            return "https://images.unsplash.com/photo-1504450758481-7338eba7524a?auto=format&fit=crop&w=800&q=80";
        if (category?.includes('festival'))
            return "https://images.unsplash.com/photo-1533174072545-7a4b6ad7a6c3?auto=format&fit=crop&w=800&q=80";
        if (category?.includes('exhibition') || category?.includes('expo'))
            return "https://images.unsplash.com/photo-1518998053901-5348d3961a04?q=80&w=1548&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D";

        return "https://images.unsplash.com/photo-1492684223066-81342ee5ff30?auto=format&fit=crop&w=800&q=80";
    };

    return (
        <Card
            hoverable
            onMouseEnter={(e) => {
                e.currentTarget.style.boxShadow = '0 8px 24px rgba(0,0,0,0.15)';
                e.currentTarget.style.transform = 'translateY(-4px)';
                e.currentTarget.style.backgroundColor = 'var(--acceloka-surface-hover)';
            }}
            onMouseLeave={(e) => {
                e.currentTarget.style.boxShadow = '0 2px 8px rgba(0,0,0,0.1)';
                e.currentTarget.style.transform = 'translateY(0)';
                e.currentTarget.style.backgroundColor = 'var(--acceloka-surface)';
            }}
            onClick={() => onSelect(ticket)}
            style={{ height: '100%', display: 'flex', flexDirection: 'column', overflow: 'hidden', borderRadius: '16px', backgroundColor: 'var(--acceloka-surface)', border: '1px solid var(--acceloka-border)', boxShadow: '0 2px 8px rgba(0,0,0,0.1)', transition: 'all 0.3s ease' }}
            styles={{ body: { flexGrow: 1, display: 'flex', flexDirection: 'column', padding: '24px', } }}
            cover={
                <div style={{ height: 160, position: 'relative', overflow: 'hidden' }}>
                    <div
                        style={{
                            position: 'absolute',
                            inset: 0,
                            backgroundImage: `url('${getCategoryImage(categoryName)}')`,
                            backgroundSize: 'cover',
                            backgroundPosition: 'center',
                            transition: 'transform 0.5s ease'
                        }}
                        className="group-hover:scale-110"
                    />

                    {/* card header */}
                    <div style={{ position: 'absolute', inset: 0, background: 'linear-gradient(to bottom, rgba(0,0,0,0.3) 0%, transparent 40%)' }} />
                    <div style={{ position: 'absolute', top: 16, left: 16 }}>
                        <Tag color={getCategoryColor(categoryName)} style={{ fontWeight: 'bold', margin: 0, padding: '2px 10px', borderRadius: '12px', border: 'none', boxShadow: '0 2px 8px rgba(0,0,0,0.15)' }}>
                            {categoryName.toUpperCase()}
                        </Tag>
                    </div>

                    {isSoldOut && (
                        <div style={{ position: 'absolute', inset: 0, backgroundColor: 'rgba(0,0,0,0.5)', display: 'flex', alignItems: 'center', justifyContent: 'center', backdropFilter: 'blur(3px)', zIndex: 10 }}>
                            <Tag color="red" style={{ transform: 'rotate(-5deg)', fontSize: 14, padding: '4px 12px', fontWeight: '900', border: '2px solid white' }}>SOLD OUT</Tag>
                        </div>
                    )}
                </div>
            }
        >

            {/* information space */}
            <Space orientation="vertical" size="middle" style={{ width: '100%', flexGrow: 1 }}>
                <Text type="secondary" style={{ fontSize: 11, fontWeight: 'bold', letterSpacing: 1, color: 'var(--acceloka-text)' }}>
                    <TagOutlined style={{ marginRight: 6 }} /> {ticketCode}
                </Text>
                <Title level={4} style={{ margin: 0, fontSize: '1.1rem', minHeight: 48, display: '-webkit-box', WebkitLineClamp: 2, WebkitBoxOrient: 'vertical', overflow: 'hidden', color: 'var(--acceloka-text)' }}>
                    {ticketName}
                </Title>
                <Space orientation="vertical" size="small" style={{ width: '100%' }}>
                    <Text type="secondary" style={{ display: 'flex', alignItems: 'center', gap: 8, color: 'var(--acceloka-muted)' }}>
                        <CalendarOutlined style={{ color: 'var(--acceloka-blue)' }} /> {eventDate}
                    </Text>
                    <Text type="secondary" style={{ display: 'flex', alignItems: 'center', gap: 8, color: 'var(--acceloka-muted)' }}>
                        <TagOutlined style={{ color: 'var(--acceloka-blue)' }} />
                        <Text type={isSoldOut ? 'danger' : isLowQuota ? 'warning' : 'success'} strong>
                            {quota}
                        </Text>
                        Tickets left
                    </Text>
                </Space>
            </Space>
            <div style={{ marginTop: 'auto', paddingTop: 20, borderTop: '1px solid var(--acceloka-border)', display: 'flex', justifyContent: 'space-between', alignItems: 'flex-end' }}>
                <div>
                    <Text type="secondary" style={{ fontSize: 10, fontWeight: 'bold', color: 'var(--acceloka-muted)' }}>PRICE</Text>
                    <Title level={3} style={{ margin: 0, color: 'var(--acceloka-blue)', fontSize: '1.3rem' }}>{formatPrice(price)}</Title>
                </div>

                {/* add button */}
                <Button
                    onMouseEnter={(e) => {
                        e.currentTarget.style.transform = 'scale(1.05)';
                        e.currentTarget.style.boxShadow = '0 4px 12px rgba(0,0,0,0.2)';
                        e.currentTarget.style.border = '1px solid var(--acceloka-text)';
                    }}
                    onMouseLeave={(e) => {
                        e.currentTarget.style.transform = 'scale(1)';
                        e.currentTarget.style.boxShadow = '0 2px 8px rgba(0,0,0,0.1)';
                        e.currentTarget.style.border = '1px solid var(--acceloka-border)';
                    }}
                    type="default"
                    icon={isSoldOut ? <ExclamationCircleOutlined /> : <PlusOutlined />}
                    disabled={isSoldOut}
                    onClick={(e) => { e.stopPropagation(); onAdd(ticket); }}
                    shape="circle"
                    size="large"
                    style={{ color: 'var(--acceloka-surface)', backgroundColor: 'var(--acceloka-blue)', border: '1px solid var(--acceloka-border)' }}
                    suppressHydrationWarning={true}
                />
            </div>
        </Card >
    );
};

export default TicketCard;